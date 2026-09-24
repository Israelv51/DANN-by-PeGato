namespace DANN.Application.Features.Tasks;

public sealed record AssignTaskItemCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    Guid? ProjectMemberId);
