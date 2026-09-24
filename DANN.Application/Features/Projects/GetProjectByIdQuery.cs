namespace DANN.Application.Features.Projects;

public sealed record GetProjectByIdQuery(
    Guid OwnerUserId,
    Guid ProjectId);
