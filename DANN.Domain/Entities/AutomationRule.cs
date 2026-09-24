using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class AutomationRule : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public AutomationTriggerType TriggerType { get; private set; }

    public AutomationActionType ActionType { get; private set; }

    public string TriggerConfiguration { get; private set; } = string.Empty;

    public string ActionConfiguration { get; private set; } = string.Empty;

    public bool IsEnabled { get; private set; } = true;

    public DateTimeOffset? LastExecutedAtUtc { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public Guid? GoalId { get; private set; }

    private AutomationRule()
    {
    }

    public AutomationRule(
        Guid ownerUserId,
        string name,
        AutomationTriggerType triggerType,
        AutomationActionType actionType,
        string description = "",
        string triggerConfiguration = "",
        string actionConfiguration = "")
    {
        OwnerUserId = ownerUserId;
        Name = name.Trim();
        Description = description.Trim();
        TriggerType = triggerType;
        ActionType = actionType;
        TriggerConfiguration = triggerConfiguration.Trim();
        ActionConfiguration = actionConfiguration.Trim();
    }

    public void UpdateDetails(
        string name,
        string description)
    {
        Name = name.Trim();
        Description = description.Trim();

        MarkAsUpdated();
    }

    public void UpdateConfiguration(
        string triggerConfiguration,
        string actionConfiguration)
    {
        TriggerConfiguration = triggerConfiguration.Trim();
        ActionConfiguration = actionConfiguration.Trim();

        MarkAsUpdated();
    }

    public void Enable()
    {
        IsEnabled = true;
        MarkAsUpdated();
    }

    public void Disable()
    {
        IsEnabled = false;
        MarkAsUpdated();
    }

    public void MarkAsExecuted()
    {
        LastExecutedAtUtc = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void LinkProject(Guid projectId)
    {
        ProjectId = projectId;
        MarkAsUpdated();
    }

    public void LinkTask(Guid taskItemId)
    {
        TaskItemId = taskItemId;
        MarkAsUpdated();
    }

    public void LinkGoal(Guid goalId)
    {
        GoalId = goalId;
        MarkAsUpdated();
    }
}
