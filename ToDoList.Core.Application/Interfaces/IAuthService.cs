using ToDoList.Core.Application.DTOs.Auth;
using ToDoList.Core.Application.Wrapper;

namespace ToDoList.Core.Application.Interfaces
{
    /// <summary>
    /// Service interface for authentication operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticates a user with username/email and password
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>Authentication response with tokens</returns>
        Task<ResponseService<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest);

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="registerRequest">Registration details</param>
        /// <returns>Registration response</returns>
        Task<ResponseService<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequest);

        /// <summary>
        /// Refreshes access token using refresh token
        /// </summary>
        /// <param name="refreshRequest">Refresh token request</param>
        /// <returns>New access token</returns>
        Task<ResponseService<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest);

        /// <summary>
        /// Revokes a refresh token
        /// </summary>
        /// <param name="token">Token to revoke</param>
        /// <param name="userId">User ID who is revoking</param>
        /// <returns>Success response</returns>
        Task<ResponseService<bool>> RevokeTokenAsync(string token, Guid userId);

        /// <summary>
        /// Logs out user by revoking all their tokens
        /// </summary>
        /// <param name="userId">User ID to logout</param>
        /// <returns>Success response</returns>
        Task<ResponseService<bool>> LogoutAsync(Guid userId);
    }
}
