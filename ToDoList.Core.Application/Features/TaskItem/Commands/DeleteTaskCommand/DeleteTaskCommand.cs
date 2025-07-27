using MediatR;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.DeleteTaskCommand
{
    public class DeleteTaskCommand : IRequest<ResponseService<DeleteTaskCommand>>
    {
        public required Guid Id { get; set; }
    }

    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ResponseService<DeleteTaskCommand>>
    {
        private readonly ITaskRepostiory _repository;
        public DeleteTaskCommandHandler(ITaskRepostiory repository)
        {
            _repository = repository;
        }
        public async Task<ResponseService<DeleteTaskCommand>> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entry = await _repository.GetByIdAsync(request.Id, cancellationToken);
                if (entry.IsSuccess)
                {
                    return ResponseService<DeleteTaskCommand>.ResponseFailure(404, ["Task not found"], null, null);
                }
                var result = await _repository.DeleteAsync(entry.Data!, cancellationToken);

                if (result.IsSuccess)
                {
                    return ResponseService<DeleteTaskCommand>.ResponseSuccess(request, "Task deleted successfully", 200);
                }

                return ResponseService<DeleteTaskCommand>.ResponseFailure(500, ["Error deleting task"], null, null);
            }
            catch (Exception ex)
            {
                return ResponseService<DeleteTaskCommand>.ResponseFailure(500, ["An error occurred while deleting the task"], ex.Message, null);
            }
        }
    }
}
