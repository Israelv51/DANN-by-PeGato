namespace DANN.Application.Features.Tasks;

public sealed record AddTaskDependencyCommand(
    Guid OwnerUserId,
    Guid TaskItemId,
    Guid DependsOnTaskItemId);
