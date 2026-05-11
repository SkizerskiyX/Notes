using Microsoft.EntityFrameworkCore;
using NoteDatabase.Abstraction;
using NoteDatabase.DbContext;
using NoteModels.Entities;

namespace NoteDatabase.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly NoteDbContext _context;

        public AuthRepository(NoteDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<RefreshToken> AddRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .SingleOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAllUserRefreshTokensAsync(Guid userId)
        {
            await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ExecuteDeleteAsync();
        }
    }
}
