namespace DANN.Application.Features.Tasks;

public sealed record GetTaskItemByIdQuery(
    Guid OwnerUserId,
    Guid TaskItemId);
