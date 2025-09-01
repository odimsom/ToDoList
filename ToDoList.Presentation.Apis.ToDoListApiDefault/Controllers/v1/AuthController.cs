using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Features.Auth.Commands;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.Common;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.v1
{
    /// <summary>
    /// API controller for authentication operations.
    /// Supports user registration, login, token refresh, and logout.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">The registration request with user details.</param>
        /// <returns>Returns the registration result with user information.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role
            };

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new { result.StatusCode, result.OperationResult });
            }

            return BadRequest(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens.
        /// </summary>
        /// <param name="request">The login request with credentials.</param>
        /// <returns>Returns authentication tokens and user information.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                UsernameOrEmail = request.UsernameOrEmail,
                Password = request.Password,
                RememberMe = request.RememberMe
            };

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new { result.StatusCode, result.OperationResult });
            }

            return Unauthorized(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>Returns new access and refresh tokens.</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var command = new RefreshTokenCommand
            {
                RefreshToken = request.RefreshToken
            };

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new { result.StatusCode, result.OperationResult });
            }

            return Unauthorized(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Logs out the current user by revoking all their refresh tokens.
        /// Requires valid JWT authentication.
        /// </summary>
        /// <returns>Returns logout confirmation.</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            var command = new LogoutCommand { UserId = userId };
            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new { result.StatusCode, result.OperationResult });
            }

            return BadRequest(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Gets the current authenticated user's information.
        /// Requires valid JWT authentication.
        /// </summary>
        /// <returns>Returns current user information.</returns>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            var firstNameClaim = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastNameClaim = User.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(new { Message = "Invalid user token" });
            }

            var currentUser = new UserDto
            {
                Id = userId,
                Username = usernameClaim ?? "",
                Email = emailClaim ?? "",
                FirstName = firstNameClaim,
                LastName = lastNameClaim,
                Role = Enum.TryParse<Core.Domain.Enums.UserRole>(roleClaim, out var role) ? role : Core.Domain.Enums.UserRole.User,
                IsActive = true // User must be active to have a valid token
            };

            return Ok(new
            {
                StatusCode = 200,
                OperationResult = new
                {
                    Data = currentUser,
                    IsSuccess = true,
                    Message = "Current user retrieved successfully"
                }
            });
        }
    }
}
