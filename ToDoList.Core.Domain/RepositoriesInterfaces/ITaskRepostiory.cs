using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces.Common;
using ToDoList.Core.Domain.Shared;

namespace ToDoList.Core.Domain.RepositoriesInterfaces
{
    public interface ITaskRepostiory : IGenericRepository<TaskItem, Guid>
    {
        Task<OperationResult<IEnumerable<TaskItem>>> GetAllAsync(TaskQuery query, CancellationToken cancellationToken);
    }
}
