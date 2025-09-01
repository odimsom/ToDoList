using System.ComponentModel.DataAnnotations;

namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for refresh token requests
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// Refresh token to use for getting new access token
        /// </summary>
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
