using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class GetTasksHandler
{
    private readonly ITaskItemRepository _taskRepository;

    public GetTasksHandler(
        ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskItemDto>> HandleAsync(
        GetTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty)
            return Array.Empty<TaskItemDto>();

        var tasks = await _taskRepository.GetByOwnerAsync(
            query.OwnerUserId,
            cancellationToken);

        return tasks
            .Where(t => t.OwnerUserId == query.OwnerUserId)
            .Select(TaskItemMapper.ToDto)
            .ToList();
    }
}
