using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class SetParentTaskHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetParentTaskHandler(
        ITaskItemRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        SetParentTaskCommand command,
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

        if (!command.ParentTaskItemId.HasValue)
        {
            taskItem.RemoveParentTask();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return ApplicationResult<TaskItemDto>.Success(
                TaskItemMapper.ToDto(taskItem));
        }

        if (command.ParentTaskItemId.Value == taskItem.Id)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "A task cannot be its own parent.");
        }

        var parentTask = await _taskRepository.GetByIdAsync(
            command.ParentTaskItemId.Value,
            cancellationToken);

        if (parentTask is null ||
            parentTask.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Parent task not found.");
        }

        if (taskItem.ProjectId != parentTask.ProjectId)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Parent task must belong to the same project.");
        }

        taskItem.SetParentTask(parentTask.Id);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }
}
