namespace DANN.Application.Features.Tasks;

public sealed record SetParentTaskCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    Guid? ParentTaskItemId);
