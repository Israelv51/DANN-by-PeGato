using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public sealed record ChangeTaskItemStatusCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    TaskItemStatus NewStatus);
