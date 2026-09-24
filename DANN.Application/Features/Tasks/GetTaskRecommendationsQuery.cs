namespace DANN.Application.Features.Tasks;

public sealed record GetTaskRecommendationsQuery(
    Guid OwnerUserId,
    DateTimeOffset NowUtc,
    int Limit = 10,
    Guid? ProjectId = null);
