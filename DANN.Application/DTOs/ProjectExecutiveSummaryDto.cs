namespace DANN.Application.DTOs;

public sealed record ProjectExecutiveSummaryDto(
    ProjectDto Project,
    ProjectProgressDto Progress,
    ProjectHealthDto Health,
    IReadOnlyList<TaskRecommendationDto> TopPriorities,
    DateTimeOffset GeneratedAtUtc);
