using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces.Common;

namespace ToDoList.Core.Domain.RepositoriesInterfaces
{
    /// <summary>
    /// Repository interface for RefreshToken entity operations
    /// </summary>
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken, Guid>
    {
        /// <summary>
        /// Gets a refresh token by token value
        /// </summary>
        /// <param name="token">The token value to search for</param>
        /// <returns>The refresh token if found, null otherwise</returns>
        Task<RefreshToken?> GetByTokenAsync(string token);

        /// <summary>
        /// Gets all active refresh tokens for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>List of active refresh tokens</returns>
        Task<IList<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId);

        /// <summary>
        /// Revokes all refresh tokens for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="revokedBy">Who revoked the tokens</param>
        /// <returns>Task</returns>
        Task RevokeAllUserTokensAsync(Guid userId, string revokedBy);

        /// <summary>
        /// Removes expired refresh tokens
        /// </summary>
        /// <returns>Task</returns>
        Task RemoveExpiredTokensAsync();
    }
}
