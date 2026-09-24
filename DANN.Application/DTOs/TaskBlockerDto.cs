using DANN.Domain.Enums;

namespace DANN.Application.DTOs;

public sealed record TaskBlockerDto(
    Guid Id,
    string Title,
    TaskItemStatus Status,
    TaskPriority Priority,
    DateTimeOffset? DueDateUtc);
