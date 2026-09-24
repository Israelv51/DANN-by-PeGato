namespace DANN.Application.Features.Tasks;

public sealed record GetTodayTasksQuery(
    Guid OwnerUserId,
    DateTimeOffset StartOfDayUtc,
    DateTimeOffset EndOfDayUtc);
