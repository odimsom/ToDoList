using ToDoList.Core.Domain.Common;

namespace ToDoList.Core.Domain.Entities
{
    /// <summary>
    /// Represents a JWT refresh token for maintaining user sessions
    /// </summary>
    public class RefreshToken : BaseEntity<Guid>
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string? RevokedBy { get; set; }
        public DateTime? RevokedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; } = null!;
    }
}
