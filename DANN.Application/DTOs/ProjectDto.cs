using DANN.Domain.Enums;

namespace DANN.Application.DTOs;

public sealed record ProjectDto(
    Guid Id,
    Guid OwnerUserId,
    Guid? ClientId,
    string Name,
    string Description,
    ProjectType Type,
    ProjectStatus Status,
    DateTimeOffset? TargetDateUtc,
    decimal? AgreedAmount,
    string Currency,
    bool IsArchived,
    DateTimeOffset? ArchivedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? UpdatedAtUtc);
