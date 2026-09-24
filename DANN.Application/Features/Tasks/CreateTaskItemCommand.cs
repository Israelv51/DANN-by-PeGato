using DANN.Domain.Enums;

namespace DANN.Application.Features.Tasks;

public sealed record CreateTaskItemCommand(
    Guid OwnerUserId,
    string Title,
    string Description = "",
    TaskPriority Priority = TaskPriority.Normal,
    DateTimeOffset? DueDateUtc = null,
    Guid? ProjectId = null);
