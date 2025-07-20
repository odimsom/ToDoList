using Microsoft.EntityFrameworkCore;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.RepositoriesInterfaces;
using ToDoList.Core.Domain.Shared;
using ToDoList.Infrastructure.Persistence.Context;
using ToDoList.Infrastructure.Persistence.Repositories.Common;

namespace ToDoList.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : GenericRepository<TaskItem, Guid>, ITaskRepostiory
    {
        private readonly ToDoListContext _context;

        public TaskRepository(ToDoListContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OperationResult<IEnumerable<TaskItem>>> GetAllAsync(TaskQuery query, CancellationToken cancellationToken)
        {
            var tasks = _context.TaskItems
                .AsNoTracking()
                .AsQueryable();

            if (query.StatusTask.HasValue)
                tasks = tasks.Where(t => t.StatusTask == query.StatusTask);

            if (query.TaskType.HasValue)
                tasks = tasks.Where(t => t.TaskType == query.TaskType);

           var queryTasks = await tasks.ToListAsync(cancellationToken);

            return OperationResult<IEnumerable<TaskItem>>.Success(queryTasks);
        }
    }
}
