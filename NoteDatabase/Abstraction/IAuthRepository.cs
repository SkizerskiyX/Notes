using NoteModels.Entities;

namespace NoteDatabase.Abstraction
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
        Task<RefreshToken> AddRefreshTokenAsync(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
        Task RevokeAllUserRefreshTokensAsync(Guid userId);
    }
}
