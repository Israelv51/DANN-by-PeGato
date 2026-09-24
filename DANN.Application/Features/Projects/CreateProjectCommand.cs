using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public sealed record CreateProjectCommand(
    Guid OwnerUserId,
    string Name,
    string Description,
    ProjectType Type,
    DateTimeOffset? TargetDateUtc = null,
    Guid? ClientId = null);
