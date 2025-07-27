using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Commands.DeleteTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Commands.UpdateTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Queries.GetAllTask;
using ToDoList.Core.Application.Features.TaskItem.Queries.GetTaskById;
using ToDoList.Core.Domain.Delegates;
using ToDoList.Core.Domain.Shared;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.Common;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.v1
{
    /// <summary>
    /// API controller for managing TaskItems.
    /// Supports CRUD operations and queries for tasks.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TaskController : BaseApiController
    {
        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="command">The task creation command.</param>
        /// <returns>Returns the created task or a validation error.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTaskCommand command)
        {
            var validation = new ValidateTaskDelegate();

            validation.OnTaskCreated = (task) =>
            {
                var diasRestantes = validation.CalculateDaysRemaining(task);
                Console.WriteLine($"Task Created: {task.Description}, EndDate is {diasRestantes} days.");
            };

            var isSuccess = await validation.ValidateTaskAsync(new Core.Domain.Entities.TaskItem
            {
                Description = command.Description,
                DueDate = command.DueDate,
            }, message =>
            {
                Console.WriteLine($"Validation faillure: {message}");
            });

            if (!isSuccess)
            {
                return BadRequest("Task validation failed.");
            }

            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Gets a task by its unique identifier.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <returns>Returns the task if found, otherwise NotFound.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = new GetTaskById { Id = id };
            var result = await Mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }

        /// <summary>
        /// Updates an existing task.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <param name="command">The update command containing new task data.</param>
        /// <returns>Returns the updated task or a validation error.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTaskCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("Task ID mismatch.");
            }
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Deletes a task by its unique identifier.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <returns>Returns the result of the delete operation.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
        {
            var command = new DeleteTaskCommand { Id = id };
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Gets all tasks matching the specified query parameters.
        /// </summary>
        /// <param name="query">Query parameters for filtering tasks.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a list of tasks or NotFound if none match.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskQuery query, CancellationToken cancellationToken)
        {
            var getAllTask = new GetAllTask { query = query };
            var result = await Mediator.Send(getAllTask, cancellationToken);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return NotFound(result);
        }
    }
}