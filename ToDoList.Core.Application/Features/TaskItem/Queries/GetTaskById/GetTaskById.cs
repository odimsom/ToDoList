using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.TaskItem.Queries.GetTaskById
{
    public class GetTaskById : IRequest<ResponseService<TaskItemDto>>
    {
        public Guid Id { get; set; }
    }
    public class GetTaskByIdHandler : IRequestHandler<GetTaskById, ResponseService<TaskItemDto>>
    {
        private readonly ITaskRepostiory _repository;
        public GetTaskByIdHandler(ITaskRepostiory repository)
        {
            _repository = repository;
        }
        public async Task<ResponseService<TaskItemDto>> Handle(GetTaskById request, CancellationToken cancellationToken)
        {
            var entry = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (!entry.IsSuccess || entry.Data == null)
            {
                return ResponseService<TaskItemDto>.ResponseFailure(404, ["Task not found"], null, null);
            }
            var taskItemDto = new TaskItemDto
            {
                Id = entry.Data.Id,
                Description = entry.Data.Description,
                DueDate = entry.Data.DueDate,
                AditionalData = entry.Data.AdditionalData!,
                StatusTask = entry.Data.StatusTask,
                TaskType = entry.Data.TaskType
            };
            return ResponseService<TaskItemDto>.ResponseSuccess(taskItemDto, "Task retrieved successfully", 200);
        }
    }

}
