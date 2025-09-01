using MediatR;
using ToDoList.Core.Application.Wrapper;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Application.Features.Auth.Commands
{
    /// <summary>
    /// Command for user logout (revoke all tokens)
    /// </summary>
    public class LogoutCommand : IRequest<ResponseService<bool>>
    {
        /// <summary>
        /// User ID to logout
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public Guid UserId { get; set; }
    }
}
