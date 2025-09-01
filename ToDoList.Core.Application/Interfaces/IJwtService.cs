using ToDoList.Core.Domain.Entities;

namespace ToDoList.Core.Application.Interfaces
{
    /// <summary>
    /// Service interface for JWT token operations
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token for the given user
        /// </summary>
        /// <param name="user">The user to generate token for</param>
        /// <returns>The generated JWT token</returns>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates a refresh token
        /// </summary>
        /// <returns>The generated refresh token</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates a JWT token and extracts user information
        /// </summary>
        /// <param name="token">The JWT token to validate</param>
        /// <returns>The user ID if valid, null otherwise</returns>
        Guid? ValidateToken(string token);

        /// <summary>
        /// Gets the remaining time until token expires
        /// </summary>
        /// <param name="token">The JWT token</param>
        /// <returns>TimeSpan until expiration, null if invalid</returns>
        TimeSpan? GetTokenRemainingTime(string token);

        /// <summary>
        /// Extracts user ID from token without validation
        /// </summary>
        /// <param name="token">The JWT token</param>
        /// <returns>User ID if extractable, null otherwise</returns>
        Guid? ExtractUserIdFromToken(string token);
    }
}
