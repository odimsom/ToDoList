using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces.Common;

namespace ToDoList.Core.Domain.RepositoriesInterfaces
{
    /// <summary>
    /// Repository interface for User entity operations
    /// </summary>
    public interface IUserRepository : IGenericRepository<User, Guid>
    {
        /// <summary>
        /// Gets a user by username
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Gets a user by email
        /// </summary>
        /// <param name="email">The email to search for</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Checks if a username already exists
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Checks if an email already exists
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>True if exists, false otherwise</returns>
        Task<bool> EmailExistsAsync(string email);
    }
}
