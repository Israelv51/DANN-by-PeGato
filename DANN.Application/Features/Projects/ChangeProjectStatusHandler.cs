using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public class ChangeProjectStatusHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeProjectStatusHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ProjectDto>> HandleAsync(
        ChangeProjectStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Owner user id is required.");
        }

        if (command.ProjectId == Guid.Empty)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Project id is required.");
        }

        var project = await _projectRepository.GetByIdAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null ||
            project.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Project not found.");
        }

        if (project.Status == command.NewStatus)
        {
            return ApplicationResult<ProjectDto>.Success(
                ProjectMapper.ToDto(project));
        }

        if (!IsValidTransition(
            project.Status,
            command.NewStatus))
        {
            return ApplicationResult<ProjectDto>.Failure(
                $"Cannot change project status from {project.Status} to {command.NewStatus}.");
        }

        switch (command.NewStatus)
        {
            case ProjectStatus.Active:
                project.Activate();
                break;

            case ProjectStatus.OnHold:
                project.Pause();
                break;

            case ProjectStatus.Completed:
                project.Complete();
                break;

            case ProjectStatus.Cancelled:
                project.Cancel();
                break;

            case ProjectStatus.Planning:
                return ApplicationResult<ProjectDto>.Failure(
                    "A project cannot be returned to Planning status.");

            default:
                return ApplicationResult<ProjectDto>.Failure(
                    "Unsupported project status.");
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<ProjectDto>.Success(
            ProjectMapper.ToDto(project));
    }

    private static bool IsValidTransition(
        ProjectStatus current,
        ProjectStatus target)
    {
        return (current, target) switch
        {
            (ProjectStatus.Planning, ProjectStatus.Active) => true,
            (ProjectStatus.Planning, ProjectStatus.Cancelled) => true,

            (ProjectStatus.Active, ProjectStatus.OnHold) => true,
            (ProjectStatus.Active, ProjectStatus.Completed) => true,
            (ProjectStatus.Active, ProjectStatus.Cancelled) => true,

            (ProjectStatus.OnHold, ProjectStatus.Active) => true,
            (ProjectStatus.OnHold, ProjectStatus.Completed) => true,
            (ProjectStatus.OnHold, ProjectStatus.Cancelled) => true,

            _ => false
        };
    }
}
