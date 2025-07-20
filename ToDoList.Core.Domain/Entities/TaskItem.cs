using ToDoList.Core.Domain.Common;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Entities
{
    public class TaskItem : BaseEntity<Guid>
    {
        public required string Description { get; set; } = string.Empty;
        public required DateTime DueDate { get; set; }
        public StatusTask StatusTask { get; set; } = StatusTask.PENDING;
        public string? AdditionalData { get; set; }
        public required TaskType TaskType { get; set; }
    }
}
