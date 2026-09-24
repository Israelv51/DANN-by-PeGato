using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public sealed record UpdateProjectCommand(
    Guid OwnerUserId,
    Guid ProjectId,
    string Name,
    string Description,
    ProjectType Type,
    DateTimeOffset? TargetDateUtc = null,
    Guid? ClientId = null);
