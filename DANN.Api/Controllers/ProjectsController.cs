using DANN.Application.Features.Projects;
using DANN.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DANN.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateProjectHandler handler,
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand(
            request.OwnerUserId,
            request.Name,
            request.Description,
            request.Type,
            request.TargetDateUtc,
            request.ClientId);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = result.Value.Id,
                ownerUserId = result.Value.OwnerUserId
            },
            result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] GetProjectsHandler handler,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        if (ownerUserId == Guid.Empty)
            return BadRequest(
                new { error = "ownerUserId is required." });

        var result = await handler.HandleAsync(
            new GetProjectsQuery(ownerUserId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        [FromServices] GetProjectByIdHandler handler,
        Guid id,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetProjectByIdQuery(ownerUserId, id),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/progress")]
    public async Task<IActionResult> GetProgress(
        [FromServices] GetProjectProgressHandler handler,
        Guid id,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetProjectProgressQuery(ownerUserId, id),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/health")]
    public async Task<IActionResult> GetHealth(
        [FromServices] GetProjectHealthHandler handler,
        Guid id,
        [FromQuery] Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetProjectHealthQuery(
                ownerUserId,
                id,
                DateTimeOffset.UtcNow),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/summary")]
    public async Task<IActionResult> GetSummary(
        [FromServices] GetProjectExecutiveSummaryHandler handler,
        Guid id,
        [FromQuery] Guid ownerUserId,
        [FromQuery] int priorityLimit = 5,
        CancellationToken cancellationToken = default)
    {
        var result = await handler.HandleAsync(
            new GetProjectExecutiveSummaryQuery(
                ownerUserId,
                id,
                DateTimeOffset.UtcNow,
                priorityLimit),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    // EDITAR PROYECTO
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        [FromServices] UpdateProjectHandler handler,
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectCommand(
            request.OwnerUserId,
            id,
            request.Name,
            request.Description,
            request.Type,
            request.TargetDateUtc,
            request.ClientId);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    // CAMBIAR ESTADO DEL PROYECTO
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromServices] ChangeProjectStatusHandler handler,
        Guid id,
        [FromBody] ChangeProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangeProjectStatusCommand(
            request.OwnerUserId,
            id,
            request.NewStatus);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    // ARCHIVAR O RESTAURAR PROYECTO
    [HttpPatch("{id:guid}/archive")]
    public async Task<IActionResult> SetArchived(
        [FromServices] SetProjectArchivedHandler handler,
        Guid id,
        [FromBody] SetProjectArchivedRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SetProjectArchivedCommand(
            request.OwnerUserId,
            id,
            request.IsArchived);

        var result = await handler.HandleAsync(
            command,
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}

public sealed record CreateProjectRequest(
    Guid OwnerUserId,
    string Name,
    string Description,
    ProjectType Type,
    DateTimeOffset? TargetDateUtc,
    Guid? ClientId);

public sealed record UpdateProjectRequest(
    Guid OwnerUserId,
    string Name,
    string Description,
    ProjectType Type,
    DateTimeOffset? TargetDateUtc,
    Guid? ClientId);

public sealed record ChangeProjectStatusRequest(
    Guid OwnerUserId,
    ProjectStatus NewStatus);

public sealed record SetProjectArchivedRequest(
    Guid OwnerUserId,
    bool IsArchived);
