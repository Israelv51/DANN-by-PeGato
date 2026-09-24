namespace DANN.Application.Features.Tasks;

public sealed record GetTaskProgressQuery(
    Guid OwnerUserId,
    Guid TaskItemId);
