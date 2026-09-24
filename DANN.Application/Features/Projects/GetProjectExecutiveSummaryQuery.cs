namespace DANN.Application.Features.Projects;

public sealed record GetProjectExecutiveSummaryQuery(
    Guid OwnerUserId,
    Guid ProjectId,
    DateTimeOffset NowUtc,
    int PriorityLimit = 5);
