using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class UpdateTaskItemHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskItemHandler(
        ITaskItemRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        UpdateTaskItemCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Owner user id is required.");
        }

        if (command.TaskItemId == Guid.Empty)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Task id is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Task title is required.");
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

        taskItem.UpdateDetails(
            command.Title.Trim(),
            command.Description);

        taskItem.ChangePriority(
            command.Priority);

        taskItem.Reschedule(
            command.DueDateUtc);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }
}
