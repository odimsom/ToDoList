using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for user information
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User unique identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Whether user is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Last login date
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Account creation date
        /// </summary>
        public DateTime CreatedDate { get; set; }
    }
}
