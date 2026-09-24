using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Projects;

public class UpdateProjectHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ProjectDto>> HandleAsync(
        UpdateProjectCommand command,
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

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Project name is required.");
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

        var normalizedName = command.Name.Trim();

        var duplicateExists =
            await _projectRepository.ExistsByNameAsync(
                command.OwnerUserId,
                normalizedName,
                command.ProjectId,
                cancellationToken);

        if (duplicateExists)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Another project with the same name already exists.");
        }

        project.Rename(normalizedName);
        project.UpdateDescription(command.Description);
        project.ChangeType(command.Type);
        project.SetTargetDate(command.TargetDateUtc);

        if (command.ClientId.HasValue)
            project.LinkClient(command.ClientId.Value);
        else
            project.RemoveClient();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<ProjectDto>.Success(
            ProjectMapper.ToDto(project));
    }
}
