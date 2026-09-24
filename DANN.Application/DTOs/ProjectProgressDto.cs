namespace DANN.Application.DTOs;

public sealed record ProjectProgressDto(
    Guid ProjectId,
    string ProjectName,
    int TotalTasks,
    int ActiveTasks,
    int CompletedTasks,
    int InProgressTasks,
    int PendingTasks,
    int CancelledTasks,
    int MeasurableTasks,
    int CompletedMeasurableTasks,
    decimal ProgressPercentage);
