using ToDoList.Core.Domain.Common;
using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Entities
{
    public class TaskItem : BaseEntity<Guid>
    {
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public StatusTask StatusTask { get; set; } = StatusTask.PENDING;
        public string? AdditionalData { get; set; }
        public TaskType TaskType { get; set; } = TaskType.LimpiezaGeneral;
    }
}
