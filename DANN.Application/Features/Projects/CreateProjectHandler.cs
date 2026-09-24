using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Entities;

namespace DANN.Application.Features.Projects;

public class CreateProjectHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<ProjectDto>> HandleAsync(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Owner user id is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Project name is required.");
        }

        var normalizedName = command.Name.Trim();

        var alreadyExists =
            await _projectRepository.ExistsByNameAsync(
                command.OwnerUserId,
                normalizedName,
                null,
                cancellationToken);

        if (alreadyExists)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "A project with the same name already exists.");
        }

        var project = new Project(
            command.OwnerUserId,
            normalizedName,
            command.Description,
            command.TargetDateUtc,
            command.ClientId,
            command.Type);

        await _projectRepository.AddAsync(
            project,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<ProjectDto>.Success(
            ProjectMapper.ToDto(project));
    }
}
