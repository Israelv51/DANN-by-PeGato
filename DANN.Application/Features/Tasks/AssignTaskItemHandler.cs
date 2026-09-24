using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;

namespace DANN.Application.Features.Tasks;

public class AssignTaskItemHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTaskItemHandler(
        ITaskItemRepository taskRepository,
        IProjectMemberRepository projectMemberRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectMemberRepository = projectMemberRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        AssignTaskItemCommand command,
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

        if (!command.ProjectMemberId.HasValue)
        {
            taskItem.UnassignMember();

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            return ApplicationResult<TaskItemDto>.Success(
                TaskItemMapper.ToDto(taskItem));
        }

        if (!taskItem.ProjectId.HasValue)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "A task must belong to a project before assigning a project member.");
        }

        var projectMember =
            await _projectMemberRepository.GetByIdAsync(
                command.ProjectMemberId.Value,
                cancellationToken);

        if (projectMember is null ||
            projectMember.OwnerUserId != command.OwnerUserId)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Project member not found.");
        }

        if (!projectMember.IsActive)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "The selected project member is not active.");
        }

        if (projectMember.ProjectId != taskItem.ProjectId.Value)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "The selected member does not belong to the task project.");
        }

        taskItem.AssignToMember(
            projectMember.Id);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }
}
