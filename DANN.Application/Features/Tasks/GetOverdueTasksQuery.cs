namespace DANN.Application.Features.Tasks;

public sealed record GetOverdueTasksQuery(
    Guid OwnerUserId,
    DateTimeOffset NowUtc);
