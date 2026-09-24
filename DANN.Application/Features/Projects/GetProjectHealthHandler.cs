using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common;
using DANN.Application.DTOs;
using DANN.Domain.Entities;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public class GetProjectHealthHandler
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITaskDependencyRepository _dependencyRepository;

    public GetProjectHealthHandler(
        IProjectRepository projectRepository,
        ITaskItemRepository taskRepository,
        ITaskDependencyRepository dependencyRepository)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _dependencyRepository = dependencyRepository;
    }

    public async Task<ApplicationResult<ProjectHealthDto>> HandleAsync(
        GetProjectHealthQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty ||
            query.ProjectId == Guid.Empty)
        {
            return ApplicationResult<ProjectHealthDto>.Failure(
                "Valid user and project identifiers are required.");
        }

        var project = await _projectRepository.GetByIdAsync(
            query.ProjectId,
            cancellationToken);

        if (project is null ||
            project.OwnerUserId != query.OwnerUserId)
        {
            return ApplicationResult<ProjectHealthDto>.Failure(
                "Project not found.");
        }

        var tasks = await _taskRepository.GetByProjectAsync(
            project.Id,
            cancellationToken);

        var ownedTasks = tasks
            .Where(t => t.OwnerUserId == query.OwnerUserId)
            .ToList();

        var activeTasks = ownedTasks
            .Where(t => t.Status != TaskItemStatus.Cancelled)
            .ToList();

        var completedTasks = activeTasks.Count(
            t => t.Status == TaskItemStatus.Completed);

        var pendingTasks = activeTasks.Count(
            t => t.Status == TaskItemStatus.Pending);

        var inProgressTasks = activeTasks.Count(
            t => t.Status == TaskItemStatus.InProgress);

        var overdueTasks = activeTasks.Count(t =>
            t.Status != TaskItemStatus.Completed &&
            t.DueDateUtc.HasValue &&
            t.DueDateUtc.Value < query.NowUtc);

        var parentTaskIds = activeTasks
            .Where(t => t.ParentTaskItemId.HasValue)
            .Select(t => t.ParentTaskItemId!.Value)
            .ToHashSet();

        var measurableTasks = activeTasks
            .Where(t => !parentTaskIds.Contains(t.Id))
            .ToList();

        var completedMeasurable = measurableTasks.Count(
            t => t.Status == TaskItemStatus.Completed);

        decimal progress = 0;

        if (measurableTasks.Count > 0)
        {
            progress = Math.Round(
                (decimal)completedMeasurable /
                measurableTasks.Count * 100,
                2);
        }

        var blockedTasks = 0;

        foreach (var task in activeTasks.Where(
            t => t.Status != TaskItemStatus.Completed))
        {
            if (await IsBlockedAsync(
                task,
                query.OwnerUserId,
                cancellationToken))
            {
                blockedTasks++;
            }
        }

        int? daysUntilTarget = null;

        if (project.TargetDateUtc.HasValue)
        {
            daysUntilTarget = (int)Math.Ceiling(
                (project.TargetDateUtc.Value - query.NowUtc).TotalDays);
        }

        var reasons = new List<string>();

        if (overdueTasks > 0)
        {
            reasons.Add(
                $"{overdueTasks} overdue task(s)");
        }

        if (blockedTasks > 0)
        {
            reasons.Add(
                $"{blockedTasks} blocked task(s)");
        }

        if (daysUntilTarget.HasValue)
        {
            if (daysUntilTarget.Value < 0)
            {
                reasons.Add(
                    "project target date has passed");
            }
            else if (daysUntilTarget.Value <= 3)
            {
                reasons.Add(
                    $"{daysUntilTarget.Value} day(s) remaining");
            }
            else if (daysUntilTarget.Value <= 7)
            {
                reasons.Add(
                    $"only {daysUntilTarget.Value} day(s) until target date");
            }
        }

        var healthStatus = DetermineHealthStatus(
            progress,
            overdueTasks,
            blockedTasks,
            daysUntilTarget);

        if (healthStatus == ProjectHealthStatus.Healthy)
        {
            reasons.Add(
                "no significant risks detected");
        }

        var result = new ProjectHealthDto(
            project.Id,
            project.Name,
            healthStatus,
            progress,
            activeTasks.Count,
            completedTasks,
            overdueTasks,
            blockedTasks,
            pendingTasks,
            inProgressTasks,
            daysUntilTarget,
            reasons);

        return ApplicationResult<ProjectHealthDto>.Success(
            result);
    }

    private async Task<bool> IsBlockedAsync(
        TaskItem task,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var dependencies =
            await _dependencyRepository.GetByTaskAsync(
                task.Id,
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
                continue;
            }

            if (requiredTask.Status != TaskItemStatus.Completed)
                return true;
        }

        return false;
    }

    private static ProjectHealthStatus DetermineHealthStatus(
        decimal progress,
        int overdueTasks,
        int blockedTasks,
        int? daysUntilTarget)
    {
        if (overdueTasks >= 3 ||
            blockedTasks >= 3 ||
            (daysUntilTarget.HasValue &&
             daysUntilTarget.Value < 0 &&
             progress < 100) ||
            (daysUntilTarget.HasValue &&
             daysUntilTarget.Value <= 3 &&
             progress < 60))
        {
            return ProjectHealthStatus.AtRisk;
        }

        if (overdueTasks > 0 ||
            blockedTasks > 0 ||
            (daysUntilTarget.HasValue &&
             daysUntilTarget.Value <= 7 &&
             progress < 80))
        {
            return ProjectHealthStatus.NeedsAttention;
        }

        return ProjectHealthStatus.Healthy;
    }
}
