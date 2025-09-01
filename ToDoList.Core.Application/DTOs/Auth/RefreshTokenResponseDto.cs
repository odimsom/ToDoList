namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for refresh token response
    /// </summary>
    public class RefreshTokenResponseDto
    {
        /// <summary>
        /// New JWT access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// New refresh token
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in UTC
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Token type (typically "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
    }
}
