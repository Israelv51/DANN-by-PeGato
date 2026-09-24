using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Projects;

public class GetProjectByIdHandler
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectByIdHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<ApplicationResult<ProjectDto>> HandleAsync(
        GetProjectByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null ||
            project.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<ProjectDto>.Failure(
                "Project not found.");
        }

        return ApplicationResult<ProjectDto>.Success(
            ProjectMapper.ToDto(project));
    }
}
