using DANN.Application.Abstractions.Persistence;
using DANN.Application.Common.Mappings;
using DANN.Application.DTOs;
using DANN.Domain.Entities;
using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public class GetTaskRecommendationsHandler
{
    private readonly ITaskItemRepository _taskRepository;
    private readonly ITaskDependencyRepository _dependencyRepository;

    public GetTaskRecommendationsHandler(
        ITaskItemRepository taskRepository,
        ITaskDependencyRepository dependencyRepository)
    {
        _taskRepository = taskRepository;
        _dependencyRepository = dependencyRepository;
    }

    public async Task<IReadOnlyList<TaskRecommendationDto>> HandleAsync(
        GetTaskRecommendationsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.OwnerUserId == Guid.Empty)
            return Array.Empty<TaskRecommendationDto>();

        var limit = Math.Clamp(query.Limit, 1, 50);

        var tasks = await _taskRepository.GetByOwnerAsync(
            query.OwnerUserId,
            cancellationToken);

        var recommendations =
            new List<TaskRecommendationDto>();

        foreach (var task in tasks)
        {
            if (task.OwnerUserId != query.OwnerUserId ||
                task.Status == TaskItemStatus.Completed ||
                task.Status == TaskItemStatus.Cancelled)
            {
                continue;
            }

            if (query.ProjectId.HasValue &&
                task.ProjectId != query.ProjectId.Value)
            {
                continue;
            }

            var blockers = await GetBlockersAsync(
                task.Id,
                query.OwnerUserId,
                cancellationToken);

            recommendations.Add(
                CreateRecommendation(
                    task,
                    query.NowUtc,
                    blockers));
        }

        return recommendations
            .OrderBy(r => r.IsBlocked)
            .ThenByDescending(r => r.Score)
            .ThenBy(r =>
                r.Task.DueDateUtc ??
                DateTimeOffset.MaxValue)
            .Take(limit)
            .ToList();
    }

    private async Task<IReadOnlyList<TaskBlockerDto>> GetBlockersAsync(
        Guid taskItemId,
        Guid ownerUserId,
        CancellationToken cancellationToken)
    {
        var dependencies =
            await _dependencyRepository.GetByTaskAsync(
                taskItemId,
                cancellationToken);

        var blockers = new List<TaskBlockerDto>();

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
            {
                blockers.Add(
                    new TaskBlockerDto(
                        requiredTask.Id,
                        requiredTask.Title,
                        requiredTask.Status,
                        requiredTask.Priority,
                        requiredTask.DueDateUtc));
            }
        }

        return blockers;
    }

    private static TaskRecommendationDto CreateRecommendation(
        TaskItem task,
        DateTimeOffset nowUtc,
        IReadOnlyList<TaskBlockerDto> blockers)
    {
        if (blockers.Count > 0)
        {
            var blockerNames = string.Join(
                ", ",
                blockers.Select(b => b.Title));

            return new TaskRecommendationDto(
                TaskItemMapper.ToDto(task),
                0,
                $"blocked by: {blockerNames}",
                true,
                blockers.Count,
                blockers);
        }

        var score = 0;
        var reasons = new List<string>();

        switch (task.Priority)
        {
            case TaskPriority.Critical:
                score += 50;
                reasons.Add("critical priority");
                break;

            case TaskPriority.High:
                score += 35;
                reasons.Add("high priority");
                break;

            case TaskPriority.Normal:
                score += 20;
                reasons.Add("normal priority");
                break;

            case TaskPriority.Low:
                score += 10;
                reasons.Add("low priority");
                break;
        }

        if (task.Status == TaskItemStatus.InProgress)
        {
            score += 10;
            reasons.Add("already in progress");
        }

        if (task.DueDateUtc.HasValue)
        {
            var remaining =
                task.DueDateUtc.Value - nowUtc;

            if (remaining < TimeSpan.Zero)
            {
                score += 40;
                reasons.Add("overdue");
            }
            else if (remaining <= TimeSpan.FromHours(24))
            {
                score += 30;
                reasons.Add("due within 24 hours");
            }
            else if (remaining <= TimeSpan.FromDays(3))
            {
                score += 20;
                reasons.Add("due within 3 days");
            }
            else if (remaining <= TimeSpan.FromDays(7))
            {
                score += 10;
                reasons.Add("due within 7 days");
            }
        }

        score = Math.Clamp(score, 0, 100);

        var reason = reasons.Count > 0
            ? string.Join(", ", reasons)
            : "no immediate urgency";

        return new TaskRecommendationDto(
            TaskItemMapper.ToDto(task),
            score,
            reason,
            false,
            0,
            Array.Empty<TaskBlockerDto>());
    }
}
