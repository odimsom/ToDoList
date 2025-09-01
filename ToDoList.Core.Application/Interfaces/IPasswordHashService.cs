namespace ToDoList.Core.Application.Interfaces
{
    /// <summary>
    /// Service interface for password hashing operations
    /// </summary>
    public interface IPasswordHashService
    {
        /// <summary>
        /// Hashes a password using a secure algorithm
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <returns>The hashed password</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against its hash
        /// </summary>
        /// <param name="password">The plain text password</param>
        /// <param name="hash">The password hash</param>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string password, string hash);
    }
}
