using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Interfaces;
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
        private readonly ICachingService _cachingService;

        public GetAllTaskHandler(ITaskRepostiory repository, ICachingService cachingService)
        {
            _repository = repository;
            _cachingService = cachingService;
        }

        public async Task<ResponseService<IEnumerable<TaskItemDto>>> Handle(GetAllTask request, CancellationToken cancellationToken)
        {
            var cacheKey = _cachingService.GenerateQueryKey(request.query);
            
            var cachedResult = await _cachingService.GetAsync<ResponseService<IEnumerable<TaskItemDto>>>(cacheKey, cancellationToken);
            if (cachedResult != null)
            {
                return cachedResult;
            }

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

                var response = ResponseService<IEnumerable<TaskItemDto>>.ResponseSuccess(taskItemsDto, "Tasks retrieved successfully", 200);
                
                // Cache the successful response for 15 minutes
                await _cachingService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15), cancellationToken);
                
                return response;
            }

            var failureResponse = ResponseService<IEnumerable<TaskItemDto>>.ResponseFailure(500, ["Error retrieving tasks"], null, null);
            return failureResponse;
        }
    }
}
