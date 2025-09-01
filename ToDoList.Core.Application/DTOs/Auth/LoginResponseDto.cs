namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for login response
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in UTC
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// User information
        /// </summary>
        public UserDto User { get; set; } = new();

        /// <summary>
        /// Token type (typically "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}
