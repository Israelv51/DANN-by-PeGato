namespace DANN.Application.Features.Projects;

public sealed record GetProjectHealthQuery(
    Guid OwnerUserId,
    Guid ProjectId,
    DateTimeOffset NowUtc);
