namespace DANN.Application.DTOs;

public sealed record ProjectHealthDto(
    Guid ProjectId,
    string ProjectName,
    ProjectHealthStatus Status,
    decimal ProgressPercentage,
    int TotalActiveTasks,
    int CompletedTasks,
    int OverdueTasks,
    int BlockedTasks,
    int PendingTasks,
    int InProgressTasks,
    int? DaysUntilTarget,
    IReadOnlyList<string> Reasons);
