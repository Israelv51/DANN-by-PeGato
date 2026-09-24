using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Projects;

public class GetProjectsHandler
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsHandler(
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyList<ProjectDto>> HandleAsync(
        GetProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty)
            return Array.Empty<ProjectDto>();

        var projects =
            await _projectRepository.GetByOwnerAsync(
                query.OwnerUserId,
                cancellationToken);

        return projects
            .Select(ProjectMapper.ToDto)
            .ToList();
    }
}
