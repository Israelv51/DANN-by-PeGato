namespace DANN.Application.Features.Tasks;

public sealed record GetTaskBlockersQuery(
    Guid OwnerUserId,
    Guid TaskItemId);
