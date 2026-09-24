using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public class GetUpcomingTasksHandler
{
    private readonly ITaskItemRepository _taskRepository;

    public GetUpcomingTasksHandler(
        ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IReadOnlyList<TaskItemDto>> HandleAsync(
        GetUpcomingTasksQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.ToUtc <= query.FromUtc)
        {
            return Array.Empty<TaskItemDto>();
        }

        var tasks = await _taskRepository.GetByOwnerAsync(
            query.OwnerUserId,
            cancellationToken);

        return tasks
            .Where(t =>
                t.OwnerUserId == query.OwnerUserId &&
                t.DueDateUtc.HasValue &&
                t.DueDateUtc.Value >= query.FromUtc &&
                t.DueDateUtc.Value <= query.ToUtc &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled)
            .OrderBy(t => t.DueDateUtc)
            .ThenByDescending(t => t.Priority)
            .Select(TaskItemMapper.ToDto)
            .ToList();
    }
}
