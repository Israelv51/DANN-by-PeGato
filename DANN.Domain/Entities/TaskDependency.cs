using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class TaskDependency : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid TaskItemId { get; private set; }

    public Guid DependsOnTaskItemId { get; private set; }

    private TaskDependency()
    {
    }

    public TaskDependency(
        Guid ownerUserId,
        Guid taskItemId,
        Guid dependsOnTaskItemId)
    {
        if (taskItemId == dependsOnTaskItemId)
            throw new ArgumentException(
                "A task cannot depend on itself.");

        OwnerUserId = ownerUserId;
        TaskItemId = taskItemId;
        DependsOnTaskItemId = dependsOnTaskItemId;
    }
}
