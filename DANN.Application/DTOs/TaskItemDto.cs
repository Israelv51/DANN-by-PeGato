using DANN.Domain.Enums;

namespace DANN.Application.DTOs;

public sealed record TaskItemDto(
    Guid Id,
    Guid OwnerUserId,
    Guid? ProjectId,
    Guid? ParentTaskItemId,
    Guid? AssigneeProjectMemberId,
    string Title,
    string Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDateUtc,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
