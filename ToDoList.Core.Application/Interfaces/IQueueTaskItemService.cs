using ToDoList.Core.Domain.Entities;

namespace ToDoList.Core.Application.Interfaces
{
    public interface IQueueTaskItemService
    {
        void AddTaskItem(TaskItem taskItem, Func<Task> action, CancellationToken cancellationToken);
    }
}
