using DANN.Domain.Enums;

namespace DANN.Application.Features.Projects;

public sealed record ChangeProjectStatusCommand(
    Guid OwnerUserId,
    Guid ProjectId,
    ProjectStatus NewStatus);
