namespace DANN.Application.Features.Tasks;

public sealed record GetUpcomingTasksQuery(
    Guid OwnerUserId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc);
