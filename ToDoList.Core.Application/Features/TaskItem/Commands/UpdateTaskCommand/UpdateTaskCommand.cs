using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.Enums;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.UpdateTaskCommand
{
    public class UpdateTaskCommand : IRequest<ResponseService<TaskItemDto>>
    {
        public required Guid Id { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string? AditionalData { get; set; }
        public StatusTask? StatusTask { get; set; }
        public TaskType? TaskType { get; set; }
        public Guid? UserId { get; set; } // Add UserId for authentication
    }

    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ResponseService<TaskItemDto>>
    {
        private readonly ITaskRepostiory _repository;
        private readonly IIdempotencyService _idempotencyService;
        private readonly ICachingService _cachingService;

        public UpdateTaskCommandHandler(
            ITaskRepostiory repository,
            IIdempotencyService idempotencyService,
            ICachingService cachingService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _idempotencyService = idempotencyService;
            _cachingService = cachingService;
        }

        public async Task<ResponseService<TaskItemDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check for idempotent response first
                var idempotencyKey = _idempotencyService.GenerateIdempotencyKey("update_task", request.Id);
                var cachedResponse = await _idempotencyService.GetIdempotentResponseAsync<TaskItemDto>(idempotencyKey, cancellationToken);
                if (cachedResponse != null)
                {
                    return cachedResponse;
                }

                var entry = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (!entry.IsSuccess || entry.Data == null)
                {
                    var notFoundResponse = ResponseService<TaskItemDto>.ResponseFailure(404, ["Task not found"], null, null);
                    return notFoundResponse;
                }

                var originalTask = entry.Data;

                // Check if the task belongs to the user (authorization)
                if (request.UserId.HasValue && originalTask.UserId != request.UserId)
                {
                    var unauthorizedResponse = ResponseService<TaskItemDto>.ResponseFailure(403, ["Unauthorized to modify this task"], null, null);
                    return unauthorizedResponse;
                }

                // Create a copy of current task data to compare
                var currentTaskDto = new TaskItemDto
                {
                    Id = originalTask.Id,
                    Description = originalTask.Description,
                    DueDate = originalTask.DueDate,
                    AditionalData = originalTask.AdditionalData!,
                    StatusTask = originalTask.StatusTask,
                    TaskType = originalTask.TaskType
                };

                // Apply updates only if values are provided
                if (request.Description != null) originalTask.Description = request.Description;
                if (request.DueDate.HasValue) originalTask.DueDate = request.DueDate.Value;
                if (request.AditionalData != null) originalTask.AdditionalData = request.AditionalData;
                if (request.StatusTask.HasValue) originalTask.StatusTask = request.StatusTask.Value;
                if (request.TaskType.HasValue) originalTask.TaskType = request.TaskType.Value;

                var updatedTaskDto = new TaskItemDto
                {
                    Id = originalTask.Id,
                    Description = originalTask.Description,
                    DueDate = originalTask.DueDate,
                    AditionalData = originalTask.AdditionalData!,
                    StatusTask = originalTask.StatusTask,
                    TaskType = originalTask.TaskType
                };

                if (!_idempotencyService.HasDataChanged(updatedTaskDto, currentTaskDto))
                {
                    var noChangeResponse = ResponseService<TaskItemDto>.ResponseSuccess(currentTaskDto, "No changes detected, task remains unchanged", 200);

                    await _idempotencyService.StoreIdempotentResponseAsync(idempotencyKey, noChangeResponse, cancellationToken);

                    return noChangeResponse;
                }

                var result = await _repository.UpdateAsync(originalTask, cancellationToken);

                if (result.IsSuccess)
                {
                    var response = ResponseService<TaskItemDto>.ResponseSuccess(
                        updatedTaskDto,
                        "Task updated successfully",
                        200);

                    await _idempotencyService.StoreIdempotentResponseAsync(idempotencyKey, response, cancellationToken);

                    await _cachingService.RemoveAsync($"task_{request.Id}", cancellationToken);
                    await _cachingService.RemoveByPatternAsync("TaskQuery_", cancellationToken);

                    return response;
                }

                var failureResponse = ResponseService<TaskItemDto>.ResponseFailure(500, ["Error updating task"], null, null);
                return failureResponse;
            }
            catch (Exception ex)
            {
                var errorResponse = ResponseService<TaskItemDto>.ResponseFailure(500, ["An error occurred while updating the task"], ex.Message, null);
                return errorResponse;
            }
        }
    }
}
