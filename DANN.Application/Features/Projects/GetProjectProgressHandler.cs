using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.DTOs;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public class GetProjectProgressHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskItemRepository _taskRepository;

    public GetProjectProgressHandler(
        IProjectRepository projectRepository,
        ITaskItemRepository taskRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
    }

    public async Task<ApplicationResult<ProjectProgressDto>> HandleAsync(
        GetProjectProgressQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.ProjectId == Guid.Empty)
        {
            return ApplicationResult<ProjectProgressDto>.Failure(
                "Valid user and project identifiers are required.");
        }

        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null ||
            project.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<ProjectProgressDto>.Failure(
                "Project not found.");
        }

        var tasks = await _taskRepository.GetByProjectAsync(
            query.ProjectId,
            cancellationToken);

        var ownedTasks = tasks
            .Where(t => t.OwnerUserId == query.OwnerUserId)
            .ToList();

        var totalTasks = ownedTasks.Count;

        var completedTasks = ownedTasks.Count(
            t => t.Status == TaskItemStatus.Completed);

        var inProgressTasks = ownedTasks.Count(
            t => t.Status == TaskItemStatus.InProgress);

        var pendingTasks = ownedTasks.Count(
            t => t.Status == TaskItemStatus.Pending);

        var cancelledTasks = ownedTasks.Count(
            t => t.Status == TaskItemStatus.Cancelled);

        var activeTasks = totalTasks - cancelledTasks;

        var parentTaskIds = ownedTasks
            .Where(t => t.ParentTaskItemId.HasValue)
            .Select(t => t.ParentTaskItemId!.Value)
            .ToHashSet();

        var measurableTasks = ownedTasks
            .Where(t =>
                !parentTaskIds.Contains(t.Id) &&
                t.Status != TaskItemStatus.Cancelled)
            .ToList();

        var completedMeasurableTasks =
            measurableTasks.Count(
                t => t.Status == TaskItemStatus.Completed);

        decimal progress = 0;

        if (measurableTasks.Count > 0)
        {
            progress = Math.Round(
                (decimal)completedMeasurableTasks /
                measurableTasks.Count * 100,
                2);
        }

        var result = new ProjectProgressDto(
            project.Id,
            project.Name,
            totalTasks,
            activeTasks,
            completedTasks,
            inProgressTasks,
            pendingTasks,
            cancelledTasks,
            measurableTasks.Count,
            completedMeasurableTasks,
            progress);

        return ApplicationResult<ProjectProgressDto>.Success(
            result);
    }
}
