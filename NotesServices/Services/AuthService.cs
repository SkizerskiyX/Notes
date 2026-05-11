using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NoteDatabase.Abstraction;
using NoteModels.Dto;
using NoteModels.Entities;
using NotesServices.Interfaces;

namespace NotesServices.Services
{
    public class AuthService : IAuthServices
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var normalizedEmail = NormalizeEmail(request.Email);
            var existingUser = await _authRepository.GetUserByEmailAsync(normalizedEmail);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new User(normalizedEmail, PasswordHasher.Hash(request.Password));
            await _authRepository.AddUserAsync(user);

            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var normalizedEmail = NormalizeEmail(request.Email);
            var user = await _authRepository.GetUserByEmailAsync(normalizedEmail);
            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            await _authRepository.RevokeAllUserRefreshTokensAsync(user.Id);
            return await IssueTokensAsync(user);
        }

        public async Task<AuthResponseDto> RefreshAsync(RefreshTokenRequestDto request)
        {
            var refreshToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.ExpiresAt <= DateTime.UtcNow || refreshToken.User == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            await _authRepository.RevokeRefreshTokenAsync(refreshToken);
            return await IssueTokensAsync(refreshToken.User);
        }

        public async Task RevokeAsync(RefreshTokenRequestDto request)
        {
            var refreshToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null)
            {
                return;
            }

            await _authRepository.RevokeRefreshTokenAsync(refreshToken);
        }

        private async Task<AuthResponseDto> IssueTokensAsync(User user)
        {
            var accessTokenMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");
            var refreshTokenDays = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7");
            var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenMinutes);

            var accessToken = GenerateJwt(user, expiresAt);
            var refreshTokenValue = GenerateRefreshToken();
            var refreshToken = new RefreshToken(refreshTokenValue, user.Id, DateTime.UtcNow.AddDays(refreshTokenDays));
            await _authRepository.AddRefreshTokenAsync(refreshToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiresAtUtc = expiresAt
            };
        }

        private string GenerateJwt(User user, DateTime expiresAtUtc)
        {
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt issuer missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt audience missing.");
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt key missing.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiresAtUtc,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            Span<byte> randomBytes = stackalloc byte[64];
            RandomNumberGenerator.Fill(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }

    internal static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, KeySize);
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public static bool Verify(string password, string hash)
        {
            var parts = hash.Split('.');
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[1]);
            var expectedKey = Convert.FromBase64String(parts[2]);
            var actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, expectedKey.Length);
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }
}
