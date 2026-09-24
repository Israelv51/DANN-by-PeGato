namespace DANN.Application.Features.Projects;

public sealed record GetProjectProgressQuery(
    Guid OwnerUserId,
    Guid ProjectId);
