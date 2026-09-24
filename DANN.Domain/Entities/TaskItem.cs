using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? ParentTaskItemId { get; private set; }

    public Guid? AssigneeProjectMemberId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public TaskItemStatus Status { get; private set; }
        = TaskItemStatus.Pending;

    public TaskPriority Priority { get; private set; }
        = TaskPriority.Normal;

    public DateTimeOffset? DueDateUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    private TaskItem()
    {
    }

    public TaskItem(
        Guid ownerUserId,
        string title,
        string description = "",
        TaskPriority priority = TaskPriority.Normal,
        DateTimeOffset? dueDateUtc = null,
        Guid? projectId = null,
        Guid? parentTaskItemId = null,
        Guid? assigneeProjectMemberId = null)
    {
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Description = description.Trim();
        Priority = priority;
        DueDateUtc = dueDateUtc;
        ProjectId = projectId;
        ParentTaskItemId = parentTaskItemId;
        AssigneeProjectMemberId = assigneeProjectMemberId;
        Status = TaskItemStatus.Pending;
    }

    public void UpdateDetails(
        string title,
        string description)
    {
        Title = title.Trim();
        Description = description.Trim();

        MarkAsUpdated();
    }

    public void Start()
    {
        Status = TaskItemStatus.InProgress;
        MarkAsUpdated();
    }

    public void Complete()
    {
        Status = TaskItemStatus.Completed;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Reopen()
    {
        Status = TaskItemStatus.Pending;
        CompletedAtUtc = null;

        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = TaskItemStatus.Cancelled;
        MarkAsUpdated();
    }

    public void ChangePriority(TaskPriority priority)
    {
        Priority = priority;
        MarkAsUpdated();
    }

    public void Reschedule(DateTimeOffset? dueDateUtc)
    {
        DueDateUtc = dueDateUtc;
        MarkAsUpdated();
    }

    public void AssignToProject(Guid projectId)
    {
        ProjectId = projectId;
        MarkAsUpdated();
    }

    public void RemoveFromProject()
    {
        ProjectId = null;
        AssigneeProjectMemberId = null;

        MarkAsUpdated();
    }

    public void AssignToMember(Guid projectMemberId)
    {
        AssigneeProjectMemberId = projectMemberId;
        MarkAsUpdated();
    }

    public void UnassignMember()
    {
        AssigneeProjectMemberId = null;
        MarkAsUpdated();
    }

    public void SetParentTask(Guid parentTaskItemId)
    {
        if (parentTaskItemId == Id)
            throw new InvalidOperationException(
                "A task cannot be its own parent.");

        ParentTaskItemId = parentTaskItemId;
        MarkAsUpdated();
    }

    public void RemoveParentTask()
    {
        ParentTaskItemId = null;
        MarkAsUpdated();
    }
}
