using MediatR;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Wrapper;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command for user login authentication
    /// </summary>
    public class LoginCommand : IRequest<ResponseService<LoginResponseDto>>
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
