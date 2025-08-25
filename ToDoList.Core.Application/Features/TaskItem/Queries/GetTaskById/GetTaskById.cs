using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Interfaces;
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
        private readonly ICachingService _cachingService;

        public GetTaskByIdHandler(ITaskRepostiory repository, ICachingService cachingService)
        {
            _repository = repository;
            _cachingService = cachingService;
        }

        public async Task<ResponseService<TaskItemDto>> Handle(GetTaskById request, CancellationToken cancellationToken)
        {
            // Generate cache key for single task retrieval
            var cacheKey = $"task_{request.Id}";
            
            // Try to get cached result first
            var cachedResult = await _cachingService.GetAsync<ResponseService<TaskItemDto>>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

            var entry = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (!entry.IsSuccess || entry.Data == null)
            {
                var notFoundResponse = ResponseService<TaskItemDto>.ResponseFailure(404, ["Task not found"], null, null);
                return notFoundResponse;
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

            var response = ResponseService<TaskItemDto>.ResponseSuccess(taskItemDto, "Task retrieved successfully", 200);
            
            await _cachingService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

            return response;
        }
    }
}
