using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for user registration
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Unique username
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User email address
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]",
            ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation
        /// </summary>
        [Required(ErrorMessage = "Password confirmation is required")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// User's first name
        /// </summary>
        [StringLength(50, ErrorMessage = "First name must not exceed 50 characters")]
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        [StringLength(50, ErrorMessage = "Last name must not exceed 50 characters")]
        public string? LastName { get; set; }

        /// <summary>
        /// User role (default: User)
        /// </summary>
        public UserRole Role { get; set; } = UserRole.User;
    }
}
