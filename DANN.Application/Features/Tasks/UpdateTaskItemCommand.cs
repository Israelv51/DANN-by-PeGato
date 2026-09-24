using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public sealed record UpdateTaskItemCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    string Title,
    string Description,
    TaskPriority Priority,
    DateTimeOffset? DueDateUtc = null);
