using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public class GetTaskProgressHandler
{
    private readonly ITaskItemRepository _taskRepository;

    public GetTaskProgressHandler(
        ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<ApplicationResult<TaskProgressDto>> HandleAsync(
        GetTaskProgressQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.TaskItemId == Guid.Empty)
        {
            return ApplicationResult<TaskProgressDto>.Failure(
                "Valid identifiers are required.");
        }

        var taskItem = await _taskRepository.GetByIdAsync(
            query.TaskItemId,
            cancellationToken);

        if (taskItem is null ||
            taskItem.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<TaskProgressDto>.Failure(
                "Task not found.");
        }

        var subtasks = await _taskRepository.GetChildrenAsync(
            taskItem.Id,
            cancellationToken);

        var ownedSubtasks = subtasks
            .Where(t => t.OwnerUserId == query.OwnerUserId)
            .ToList();

        var total = ownedSubtasks.Count;

        var completed = ownedSubtasks.Count(
            t => t.Status == TaskItemStatus.Completed);

        var inProgress = ownedSubtasks.Count(
            t => t.Status == TaskItemStatus.InProgress);

        var pending = ownedSubtasks.Count(
            t => t.Status == TaskItemStatus.Pending);

        var cancelled = ownedSubtasks.Count(
            t => t.Status == TaskItemStatus.Cancelled);

        var activeSubtasks = total - cancelled;

        decimal progress = 0;

        if (activeSubtasks > 0)
        {
            progress = Math.Round(
                (decimal)completed / activeSubtasks * 100,
                2);
        }

        var result = new TaskProgressDto(
            taskItem.Id,
            total,
            completed,
            inProgress,
            pending,
            cancelled,
            progress);

        return ApplicationResult<TaskProgressDto>.Success(
            result);
    }
}
