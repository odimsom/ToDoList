using MediatR;
using ToDoList.Core.Application.DTOs.TaskItem;
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
    }
    
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, ResponseService<TaskItemDto>>
    {
        private readonly ITaskRepostiory _repository;

        public UpdateTaskCommandHandler(ITaskRepostiory repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        async Task<ResponseService<TaskItemDto>> IRequestHandler<UpdateTaskCommand, ResponseService<TaskItemDto>>.Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entry = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (!entry.IsSuccess || entry.Data == null)
                {
                    return ResponseService<TaskItemDto>.ResponseFailure(404, ["Task not found"], null, null);
                }
                var result = await _repository.UpdateAsync(entry.Data, cancellationToken);

                if (result.IsSuccess)
                {
                    return ResponseService<TaskItemDto>.ResponseSuccess(
                        new TaskItemDto
                        {
                            Id = result.Data!.Id,
                            Description = request.Description ?? result.Data.Description,
                            DueDate = request.DueDate ?? result.Data.DueDate,
                            AditionalData = request.AditionalData ?? result.Data.AdditionalData!,
                            StatusTask = request.StatusTask ?? result.Data.StatusTask,
                            TaskType = request.TaskType ?? result.Data.TaskType
                        },
                        "Task updated successfully",
                        200);
                }

                return ResponseService<TaskItemDto>.ResponseFailure(500, ["Error updating task"], null, null);
            }
            catch (Exception ex)
            {
                return ResponseService<TaskItemDto>.ResponseFailure(500, ["An error occurred while updating the task"], ex.Message, null);
            }
    }   }
}
