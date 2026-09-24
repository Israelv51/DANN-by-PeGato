namespace DANN.Application.Features.Tasks;

public sealed record GetProjectTasksQuery(
    Guid OwnerUserId,
    Guid ProjectId);
