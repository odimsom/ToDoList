using MediatR;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Wrapper;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command for refreshing JWT tokens
    /// </summary>
    public class RefreshTokenCommand : IRequest<ResponseService<RefreshTokenResponseDto>>
    {
        /// <summary>
        /// Refresh token to use for getting new access token
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
