using Microsoft.AspNetCore.Identity;
using ToDoList.Core.Application.Interfaces;

namespace ToDoList.Infrastructure.Shared.Services
{
    /// <summary>
    /// Service implementation for password hashing using BCrypt
    /// </summary>
    public class PasswordHashService : IPasswordHashService
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHashService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Hash cannot be null or empty", nameof(hash));

            var result = _passwordHasher.VerifyHashedPassword(null!, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
