using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Projects;

public class SetProjectArchivedHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetProjectArchivedHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ProjectDto>> HandleAsync(
        SetProjectArchivedCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty ||
            command.ProjectId == Guid.Empty)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "User and project identifiers are required.");
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

        if (command.IsArchived)
            project.Archive();
        else
            project.Restore();

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<ProjectDto>.Success(
            ProjectMapper.ToDto(project));
    }
}
