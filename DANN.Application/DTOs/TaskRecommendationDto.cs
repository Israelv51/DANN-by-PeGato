namespace DANN.Application.DTOs;

public sealed record TaskRecommendationDto(
    TaskItemDto Task,
    int Score,
    string Reason,
    bool IsBlocked,
    int BlockerCount,
    IReadOnlyList<TaskBlockerDto> Blockers);
