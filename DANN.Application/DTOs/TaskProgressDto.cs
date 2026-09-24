namespace DANN.Application.DTOs;

public sealed record TaskProgressDto(
    Guid TaskItemId,
    int TotalSubtasks,
    int CompletedSubtasks,
    int InProgressSubtasks,
    int PendingSubtasks,
    int CancelledSubtasks,
    decimal ProgressPercentage);
