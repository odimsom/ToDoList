using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.Entities.Types;

namespace ToDoList.Core.Application.Factories
{
    public interface ITaskItemFactory
    {
        /// <summary>
        /// Creates a new task item with the specified parameters.
        /// </summary>
        /// <param name="taskItem">Task.</param>
        /// <returns>A new instance of a task item.</returns>
        BaseTypesTask CreateTask(TaskItem taskItem);
    }
}
