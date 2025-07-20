using ToDoList.Core.Domain.Enums;

namespace ToDoList.Core.Domain.Entities.Types
{
    public abstract class BaseTypesTask
    {
        public abstract TaskType GetTaskType();
    }
}
