using AutoMapper;
using MediatR;
using System.Net;
using System.Text.Json;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.Enums;
using ToDoList.Core.Domain.RepositoriesInterfaces;

namespace ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand
{
    public class CreateTaskCommand : IRequest<ResponseService<CreateTaskCommand>>
    {
        public string Description { get; set; } = string.Empty;
        public StatusTask StatusTask { get; set; }
        public DateTime DueDate { get; set; }
        public string AditionalData { get; set; } = string.Empty;
        public TaskType TaskType { get; set; }
        public Guid? UserId { get; set; } // Add UserId for authentication
    }

    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ResponseService<CreateTaskCommand>>
    {
        private readonly ITaskRepostiory _repository;
        private readonly IMapper _mapper;
        private readonly IQueueTaskItemService _queueTaskItemService;
        public CreateTaskCommandHandler(ITaskRepostiory repository, IMapper mapper, IQueueTaskItemService queueTaskItemService)
        {
            _repository = repository;
            _mapper = mapper;
            _queueTaskItemService = queueTaskItemService;
        }

        public async Task<ResponseService<CreateTaskCommand>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.AditionalData))
                {
                    string json = JsonSerializer.Serialize(request.AditionalData);
                    request.AditionalData = json;
                }
                Domain.Entities.TaskItem taskItem = new Domain.Entities.TaskItem
                {
                    Description = request.Description,
                    DueDate = request.DueDate,
                    TaskType = request.TaskType,
                    AdditionalData = request.AditionalData,
                    UserId = request.UserId // Set the user ID
                };
                var tcs = new TaskCompletionSource<Guid>();

                _queueTaskItemService.AddTaskItem(taskItem, async () =>
                {
                    var result = await _repository.AddAsync(taskItem, cancellationToken);
                    tcs.SetResult(result.Data!.Id);
                }, cancellationToken);


                var result = await tcs.Task;
                return ResponseService<CreateTaskCommand>.ResponseSuccess(request, "Task created successfully", (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return ResponseService<CreateTaskCommand>.ResponseFailure(
                    (int)HttpStatusCode.InternalServerError,
                    ["An error occurred while creating the task"],
                    ex.Message,
                    null);
            }
        }
    }
}
