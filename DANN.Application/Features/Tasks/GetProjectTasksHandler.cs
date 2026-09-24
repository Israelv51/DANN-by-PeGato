using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class GetProjectTasksHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;

    public GetProjectTasksHandler(
        ITaskItemRepository taskRepository,
        IProjectRepository projectRepository)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<TaskItemDto>>> HandleAsync(
        GetProjectTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.ProjectId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<TaskItemDto>>.Failure(
                "User and project identifiers are required.");
        }

        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null ||
            project.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<IReadOnlyList<TaskItemDto>>.Failure(
                "Project not found.");
        }

        var tasks = await _taskRepository.GetByProjectAsync(
            query.ProjectId,
            cancellationToken);

        var result = tasks
            .Where(t => t.OwnerUserId == query.OwnerUserId)
            .Select(TaskItemMapper.ToDto)
            .ToList();

        return ApplicationResult<IReadOnlyList<TaskItemDto>>.Success(
            result);
    }
}
