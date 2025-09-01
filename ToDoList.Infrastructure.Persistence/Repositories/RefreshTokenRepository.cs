using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces;
using ToDoList.Core.Domain.Shared;
using ToDoList.Infrastructure.Persistence.Context;
using ToDoList.Infrastructure.Persistence.Repositories.Common;

namespace ToDoList.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository implementation for RefreshToken entity operations
    /// </summary>
    public class RefreshTokenRepository : GenericRepository<RefreshToken, Guid>, IRefreshTokenRepository
    {
        private readonly ToDoListContext _context;

        public RefreshTokenRepository(ToDoListContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IList<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            return await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId &&
                           !rt.IsRevoked &&
                           rt.ExpirationDate > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string revokedBy)
        {
            var tokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedBy = revokedBy;
                token.RevokedDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveExpiredTokensAsync()
        {
            var expiredTokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.ExpirationDate <= DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync();

            _context.Set<RefreshToken>().RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        // Override methods to handle the repository pattern correctly
        public async Task<OperationResult<RefreshToken>> AddAsync(RefreshToken entity)
        {
            return await base.AddAsync(entity, CancellationToken.None);
        }

        public async Task<OperationResult<RefreshToken>> UpdateAsync(RefreshToken entity)
        {
            return await base.UpdateAsync(entity, CancellationToken.None);
        }

        public async Task<OperationResult<RefreshToken>> DeleteAsync(Guid id)
        {
            var entity = await _context.Set<RefreshToken>().FindAsync(id);
            if (entity == null)
                return OperationResult<RefreshToken>.Failure(new List<string> { "RefreshToken not found" });

            return await base.DeleteAsync(entity, CancellationToken.None);
        }

        public async Task<RefreshToken?> GetByIdAsync(Guid id)
        {
            return await _context.Set<RefreshToken>().FindAsync(id);
        }
    }
}
