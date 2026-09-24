using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class GetTaskItemByIdHandler
{
    private readonly ITaskItemRepository _taskRepository;

    public GetTaskItemByIdHandler(
        ITaskItemRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        GetTaskItemByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.TaskItemId == Guid.Empty)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "User and task identifiers are required.");
        }

        var taskItem = await _taskRepository.GetByIdAsync(
            query.TaskItemId,
            cancellationToken);

        if (taskItem is null ||
            taskItem.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Task not found.");
        }

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }
}
