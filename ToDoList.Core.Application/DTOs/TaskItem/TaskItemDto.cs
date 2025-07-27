using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Application.DTOs.TaskItem
{
    public class TaskItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public StatusTask StatusTask { get; set; }
        public DateTime DueDate { get; set; }
        public string AditionalData { get; set; }
        public TaskType TaskType { get; set; }
    }
}
