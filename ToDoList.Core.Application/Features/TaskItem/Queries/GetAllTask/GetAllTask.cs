using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.RepositoriesInterfaces;
using ToDoList.Core.Domain.Shared;

namespace ToDoList.Core.Application.Features.TaskItem.Queries.GetAllTask
{
    public class GetAllTask : IRequest<ResponseService<IEnumerable<TaskItemDto>>>
    {
        public TaskQuery query { get; set; }
    }

    public class GetAllTaskHandler : IRequestHandler<GetAllTask, ResponseService<IEnumerable<TaskItemDto>>>
    {
        private readonly ITaskRepostiory _repository;
        public GetAllTaskHandler(ITaskRepostiory repository)
        {
            _repository = repository;
        }
        public async Task<ResponseService<IEnumerable<TaskItemDto>>> Handle(GetAllTask request, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(request.query, cancellationToken);
            if (result.IsSuccess)
            {
                var taskItemsDto = result.Data!.Select(task => new TaskItemDto
                {
                    Id = task.Id,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    AditionalData = task.AdditionalData!,
                    StatusTask = task.StatusTask,
                    TaskType = task.TaskType
                });
                return ResponseService<IEnumerable<TaskItemDto>>.ResponseSuccess(taskItemsDto, "Tasks retrieved successfully", 200);
            }
            return ResponseService<IEnumerable<TaskItemDto>>.ResponseFailure(500, ["Error retrieving tasks"], null, null);
        }
    }
}
