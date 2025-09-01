using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ToDoList.Core.Application.DTOs.TaskItem;
using ToDoList.Core.Application.Features.TaskItem.Commands.CreateTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Commands.DeleteTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Commands.UpdateTaskCommand;
using ToDoList.Core.Application.Features.TaskItem.Queries.GetAllTask;
using ToDoList.Core.Application.Features.TaskItem.Queries.GetTaskById;
using ToDoList.Core.Application.Interfaces;
using ToDoList.Core.Application.Wrapper;
using ToDoList.Core.Domain.Delegates;
using ToDoList.Core.Domain.Entities;
using ToDoList.Core.Domain.Shared;
using ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.Common;

namespace ToDoList.Presentation.Apis.ToDoListApiDefault.Controllers.v1
{
    /// <summary>
    /// API controller for managing TaskItems.
    /// Supports CRUD operations and queries for tasks with idempotency and caching.
    /// Requires JWT authentication.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all endpoints
    public class TaskController : BaseApiController
    {
        private readonly IIdempotencyService _idempotencyService;

        public TaskController(IIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }

        /// <summary>
        /// Creates a new task for the authenticated user.
        /// </summary>
        /// <param name="command">The task creation command.</param>
        /// <param name="idempotencyKey">Optional idempotency key for duplicate request protection.</param>
        /// <returns>Returns the created task or a validation error.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] CreateTaskCommand command,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            // Set the user ID for the task
            command.UserId = userId;

            idempotencyKey ??= _idempotencyService.GenerateIdempotencyKey("create_task", command);

            var cachedResponse = await _idempotencyService.GetIdempotentResponseAsync<CreateTaskCommand>(idempotencyKey);
            if (cachedResponse != null)
            {
                Response.Headers.Append("Idempotency-Replayed", "true");
                return Ok(new { cachedResponse.StatusCode, cachedResponse.OperationResult });
            }

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
                UserId = userId
            }, message =>
            {
                Console.WriteLine($"Validation faillure: {message}");
            });

            if (!isSuccess)
            {
                return BadRequest("Task validation failed.");
            }

            var result = await Mediator.Send(command);

            await _idempotencyService.StoreIdempotentResponseAsync(idempotencyKey, result);

            Response.Headers.Append("Idempotency-Key", idempotencyKey);
            return Ok(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Gets a task by its unique identifier (cached).
        /// Only returns tasks belonging to the authenticated user.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <returns>Returns the task if found and owned by user, otherwise NotFound.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            var query = new GetTaskById { Id = id, UserId = userId };
            var result = await Mediator.Send(query);

            if (result.OperationResult.IsSuccess)
            {
                Response.Headers.Append("Cache-Control", "public, max-age=1800");
                return Ok(new { result.StatusCode, result.OperationResult });
            }
            return NotFound(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Updates an existing task (idempotent with change detection).
        /// Only allows updating tasks belonging to the authenticated user.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <param name="command">The update command containing new task data.</param>
        /// <param name="idempotencyKey">Optional idempotency key for duplicate request protection.</param>
        /// <returns>Returns the updated task or a validation error.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put(
            Guid id,
            [FromBody] UpdateTaskCommand command,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null)
        {
            if (id != command.Id)
            {
                return BadRequest("Task ID mismatch.");
            }

            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            // Set the user ID for authorization check
            command.UserId = userId;

            idempotencyKey ??= _idempotencyService.GenerateIdempotencyKey("update_task", command.Id);

            var result = await Mediator.Send(command);

            var cachedResponse = await _idempotencyService.GetIdempotentResponseAsync<TaskItemDto>(idempotencyKey);
            if (cachedResponse != null && result.Message?.Contains("No changes detected") == true)
            {
                Response.Headers.Append("Idempotency-Replayed", "true");
            }

            Response.Headers.Append("Idempotency-Key", idempotencyKey);
            return Ok(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Deletes a task by its unique identifier (idempotent).
        /// Only allows deleting tasks belonging to the authenticated user.
        /// </summary>
        /// <param name="id">The task's unique identifier.</param>
        /// <param name="idempotencyKey">Optional idempotency key for duplicate request protection.</param>
        /// <returns>Returns the result of the delete operation.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            [Required] Guid id,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey = null)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            idempotencyKey ??= _idempotencyService.GenerateIdempotencyKey("delete_task", id);

            var cachedResponse = await _idempotencyService.GetIdempotentResponseAsync<DeleteTaskCommand>(idempotencyKey);
            if (cachedResponse != null)
            {
                Response.Headers.Append("Idempotency-Replayed", "true");
                return Ok(new { cachedResponse.StatusCode, cachedResponse.OperationResult });
            }

            var command = new DeleteTaskCommand { Id = id, UserId = userId };
            var result = await Mediator.Send(command);

            await _idempotencyService.StoreIdempotentResponseAsync(idempotencyKey, result);

            Response.Headers.Append("Idempotency-Key", idempotencyKey);
            return Ok(new { result.StatusCode, result.OperationResult });
        }

        /// <summary>
        /// Gets all tasks matching the specified query parameters (cached).
        /// Only returns tasks belonging to the authenticated user.
        /// </summary>
        /// <param name="query">Query parameters for filtering tasks.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a list of tasks or NotFound if none match.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskQuery query, CancellationToken cancellationToken)
        {
            // Get current user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return BadRequest(new { Message = "Invalid user token" });
            }

            var getAllTask = new GetAllTask { query = query, UserId = userId };
            var result = await Mediator.Send(getAllTask, cancellationToken);

            Response.Headers.Append("Cache-Control", "public, max-age=900");

            if (result.OperationResult.IsSuccess)
            {
                return Ok(new { result.StatusCode, result.OperationResult });
            }
            return NotFound(new { result.StatusCode, result.OperationResult });
        }
    }
}