using DANN.Application.Features.Tasks;
using DANN.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DANN.Api.Controllers;

[ApiController]
[Route("api")]
public class TasksController : ControllerBase
{
    [HttpPost("tasks")]
    public async Task<IActionResult> Create(
        [FromServices] CreateTaskItemHandler handler,
        [FromBody] CreateTaskItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTaskItemCommand(
            request.OwnerUserId,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDateUtc,
            request.ProjectId);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id, ownerUserId = request.OwnerUserId }, result.Value);
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> GetAll(
        [FromServices] GetTasksHandler handler,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        if (ownerUserId == Guid.Empty)
        {
            return BadRequest(new { error = "User identifier is required." });
        }

        var result = await handler.HandleAsync(
            new GetTasksQuery(ownerUserId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("tasks/{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] GetTaskItemByIdHandler handler,
        Guid id,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetTaskItemByIdQuery(ownerUserId, id),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error == "Task not found.")
            {
                return NotFound(new { error = result.Error });
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("projects/{projectId:guid}/tasks")]
    public async Task<IActionResult> GetByProject(
        [FromServices] GetProjectTasksHandler handler,
        Guid projectId,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetProjectTasksQuery(ownerUserId, projectId),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            if (result.Error == "Project not found.")
            {
                return NotFound(new { error = result.Error });
            }

            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPatch("tasks/{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromServices] ChangeTaskItemStatusHandler handler,
        Guid id,
        [FromBody] ChangeTaskStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeTaskItemStatusCommand(
            request.OwnerUserId,
            id,
            request.NewStatus);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return Ok(result.Value);
    }

    [HttpPost("tasks/{id:guid}/dependencies")]
    public async Task<IActionResult> AddDependency(
        [FromServices] AddTaskDependencyHandler handler,
        Guid id,
        [FromBody] AddTaskDependencyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTaskDependencyCommand(
            request.OwnerUserId,
            id,
            request.DependsOnTaskItemId);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                error = result.Error
            });
        }

        return Ok(result);
    }
}

public sealed record CreateTaskItemRequest(
    Guid OwnerUserId,
    string Title,
    string Description,
    TaskPriority Priority,
    DateTimeOffset? DueDateUtc,
    Guid? ProjectId);

public sealed record ChangeTaskStatusRequest(
    Guid OwnerUserId,
    TaskItemStatus NewStatus);

public sealed record AddTaskDependencyRequest(
    Guid OwnerUserId,
    Guid DependsOnTaskItemId);




