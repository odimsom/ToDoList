using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for user login requests
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Username or email for login
        /// </summary>
        [Required(ErrorMessage = "Username or email is required")]
        [StringLength(100, ErrorMessage = "Username or email must not exceed 100 characters")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Remember user session for extended period
        /// </summary>
        public bool RememberMe { get; set; } = false;
    }
}
