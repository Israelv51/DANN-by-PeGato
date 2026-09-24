namespace DANN.Application.Features.Projects;

public sealed record SetProjectArchivedCommand(
    Guid OwnerUserId,
    Guid ProjectId,
    bool IsArchived);
