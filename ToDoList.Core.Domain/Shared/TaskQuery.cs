using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Shared
{
    public class TaskQuery
    {
        public StatusTask? StatusTask { get; set; }
        public TaskType? TaskType { get; set; }
        public Guid? UserId { get; set; } // Add UserId for filtering by user
    }
}
