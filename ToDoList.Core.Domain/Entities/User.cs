using ToDoList.Core.Domain.Common;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system with authentication capabilities
    /// </summary>
    public class User : BaseEntity<Guid>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
