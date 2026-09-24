using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Entities;

namespace DANN.Application.Features.Tasks;

public class CreateTaskItemHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskItemHandler(
        ITaskItemRepository taskRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApplicationResult<TaskItemDto>> HandleAsync(
        CreateTaskItemCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.OwnerUserId == Guid.Empty)
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Owner user id is required.");
        }

        if (string.IsNullOrWhiteSpace(command.Title))
        {
            return ApplicationResult<TaskItemDto>.Failure(
                "Task title is required.");
        }

        if (command.ProjectId.HasValue)
        {
            var project = await _projectRepository.GetByIdAsync(
                command.ProjectId.Value,
                cancellationToken);

            if (project is null ||
                project.OwnerUserId != command.OwnerUserId)
            {
                return ApplicationResult<TaskItemDto>.Failure(
                    "Project not found.");
            }

            if (project.IsArchived)
            {
                return ApplicationResult<TaskItemDto>.Failure(
                    "Tasks cannot be added to an archived project.");
            }
        }

        var taskItem = new TaskItem(
            command.OwnerUserId,
            command.Title.Trim(),
            command.Description,
            command.Priority,
            command.DueDateUtc,
            command.ProjectId);

        await _taskRepository.AddAsync(
            taskItem,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return ApplicationResult<TaskItemDto>.Success(
            TaskItemMapper.ToDto(taskItem));
    }
}
