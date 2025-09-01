namespace ToDoList.Core.Application.DTOs.Auth
{
    /// <summary>
    /// Data transfer object for registration response
    /// </summary>
    public class RegisterResponseDto
    {
        /// <summary>
        /// User information
        /// </summary>
        public UserDto User { get; set; } = new();

        /// <summary>
        /// Registration success message
        /// </summary>
        public string Message { get; set; } = "User registered successfully";
    }
}
