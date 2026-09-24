using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public class ChangeTaskItemStatusHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITaskDependencyRepository _dependencyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTaskItemStatusHandler(
        ITaskItemRepository taskRepository,
        ITaskDependencyRepository dependencyRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _dependencyRepository = dependencyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        ChangeTaskItemStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty ||
            command.TaskItemId == Guid.Empty)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "User and task identifiers are required.");
        }

        var taskItem = await _taskRepository.GetByIdAsync(
            command.TaskItemId,
            cancellationToken);

        if (taskItem is null ||
            taskItem.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Task not found.");
        }

        if (taskItem.Status == command.NewStatus)
        {
            return ApplicationResult<TaskItemDto>.Success(
                TaskItemMapper.ToDto(taskItem));
        }

        if (!IsValidTransition(
            taskItem.Status,
            command.NewStatus))
        {
            return ApplicationResult<TaskItemDto>.Failure(
                $"Cannot change task status from {taskItem.Status} to {command.NewStatus}.");
        }

        if (command.NewStatus == TaskItemStatus.InProgress ||
            command.NewStatus == TaskItemStatus.Completed)
        {
            var dependencyCheck =
                await ValidateDependenciesAsync(
                    taskItem.Id,
                    command.OwnerUserId,
                    cancellationToken);

            if (!dependencyCheck.IsSuccess)
            {
                return ApplicationResult<TaskItemDto>.Failure(
                    dependencyCheck.Error);
            }
        }

        if (command.NewStatus == TaskItemStatus.Completed)
        {
            var subtaskCheck =
                await ValidateSubtasksAsync(
                    taskItem.Id,
                    command.OwnerUserId,
                    cancellationToken);

            if (!subtaskCheck.IsSuccess)
            {
                return ApplicationResult<TaskItemDto>.Failure(
                    subtaskCheck.Error);
            }
        }

        switch (command.NewStatus)
        {
            case TaskItemStatus.Pending:
                taskItem.Reopen();
                break;

            case TaskItemStatus.InProgress:
                taskItem.Start();
                break;

            case TaskItemStatus.Completed:
                taskItem.Complete();
                break;

            case TaskItemStatus.Cancelled:
                taskItem.Cancel();
                break;

            default:
                return ApplicationResult<TaskItemDto>.Failure(
                    "Unsupported task status.");
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }

    private async Task<ApplicationResult<bool>> ValidateDependenciesAsync(
        Guid taskItemId,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var dependencies =
            await _dependencyRepository.GetByTaskAsync(
                taskItemId,
                cancellationToken);

        foreach (var dependency in dependencies)
        {
            var requiredTask =
                await _taskRepository.GetByIdAsync(
                    dependency.DependsOnTaskItemId,
                    cancellationToken);

            if (requiredTask is null ||
                requiredTask.OwnerUserId != ownerUserId)
            {
                return ApplicationResult<bool>.Failure(
                    "A required dependency could not be found.");
            }

            if (requiredTask.Status != TaskItemStatus.Completed)
            {
                return ApplicationResult<bool>.Failure(
                    $"Task cannot continue because '{requiredTask.Title}' is not completed.");
            }
        }

        return ApplicationResult<bool>.Success(true);
    }

    private async Task<ApplicationResult<bool>> ValidateSubtasksAsync(
        Guid taskItemId,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var subtasks = await _taskRepository.GetChildrenAsync(
            taskItemId,
            cancellationToken);

        var unfinishedSubtasks = subtasks
            .Where(t =>
                t.OwnerUserId == ownerUserId &&
                t.Status != TaskItemStatus.Completed &&
                t.Status != TaskItemStatus.Cancelled)
            .ToList();

        if (unfinishedSubtasks.Count == 0)
        {
            return ApplicationResult<bool>.Success(true);
        }

        var names = string.Join(
            ", ",
            unfinishedSubtasks.Select(t => t.Title));

        return ApplicationResult<bool>.Failure(
            $"Task cannot be completed because these subtasks are still active: {names}.");
    }

    private static bool IsValidTransition(
        TaskItemStatus current,
        TaskItemStatus target)
    {
        return (current, target) switch
        {
            (TaskItemStatus.Pending, TaskItemStatus.InProgress) => true,
            (TaskItemStatus.Pending, TaskItemStatus.Completed) => true,
            (TaskItemStatus.Pending, TaskItemStatus.Cancelled) => true,

            (TaskItemStatus.InProgress, TaskItemStatus.Pending) => true,
            (TaskItemStatus.InProgress, TaskItemStatus.Completed) => true,
            (TaskItemStatus.InProgress, TaskItemStatus.Cancelled) => true,

            (TaskItemStatus.Completed, TaskItemStatus.Pending) => true,

            (TaskItemStatus.Cancelled, TaskItemStatus.Pending) => true,

            _ => false
        };
    }
}
