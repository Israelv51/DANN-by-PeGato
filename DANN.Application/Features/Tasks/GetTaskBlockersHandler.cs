using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public class GetTaskBlockersHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITaskDependencyRepository _dependencyRepository;

    public GetTaskBlockersHandler(
        ITaskItemRepository taskRepository,
        ITaskDependencyRepository dependencyRepository)
    {
        _taskRepository = taskRepository;
        _dependencyRepository = dependencyRepository;
    }

    public async Task<ApplicationResult<IReadOnlyList<TaskItemDto>>> HandleAsync(
        GetTaskBlockersQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.TaskItemId == Guid.Empty)
        {
            return ApplicationResult<IReadOnlyList<TaskItemDto>>.Failure(
                "Valid identifiers are required.");
        }

        var taskItem = await _taskRepository.GetByIdAsync(
            query.TaskItemId,
            cancellationToken);

        if (taskItem is null ||
            taskItem.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<IReadOnlyList<TaskItemDto>>.Failure(
                "Task not found.");
        }

        var dependencies =
            await _dependencyRepository.GetByTaskAsync(
                taskItem.Id,
                cancellationToken);

        var blockers = new List<TaskItemDto>();

        foreach (var dependency in dependencies)
        {
            var requiredTask =
                await _taskRepository.GetByIdAsync(
                    dependency.DependsOnTaskItemId,
                    cancellationToken);

            if (requiredTask is null ||
                requiredTask.OwnerUserId != query.OwnerUserId)
            {
                continue;
            }

            if (requiredTask.Status != TaskItemStatus.Completed)
            {
                blockers.Add(
                    TaskItemMapper.ToDto(requiredTask));
            }
        }

        return ApplicationResult<IReadOnlyList<TaskItemDto>>.Success(
            blockers);
    }
}
