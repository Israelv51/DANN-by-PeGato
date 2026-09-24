namespace DANN.Application.Features.Tasks;

public sealed record RemoveTaskDependencyCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    Guid DependsOnTaskItemId);
