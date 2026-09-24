using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Goal : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? ParentGoalId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public GoalStatus Status { get; private set; }
        = GoalStatus.NotStarted;

    public DateTimeOffset? TargetDateUtc { get; private set; }

    public decimal CurrentValue { get; private set; }

    public decimal TargetValue { get; private set; } = 100;

    public string Unit { get; private set; } = "%";

    private Goal()
    {
    }

    public Goal(
        Guid ownerUserId,
        string title,
        string description = "",
        DateTimeOffset? targetDateUtc = null,
        decimal targetValue = 100,
        string unit = "%")
    {
        if (targetValue <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(targetValue),
                "Target value must be greater than zero.");

        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Description = description.Trim();
        TargetDateUtc = targetDateUtc;
        TargetValue = targetValue;
        Unit = unit.Trim();
    }

    public void Start()
    {
        Status = GoalStatus.Active;
        MarkAsUpdated();
    }

    public void Pause()
    {
        Status = GoalStatus.Paused;
        MarkAsUpdated();
    }

    public void Resume()
    {
        Status = GoalStatus.Active;
        MarkAsUpdated();
    }

    public void Complete()
    {
        CurrentValue = TargetValue;
        Status = GoalStatus.Completed;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = GoalStatus.Cancelled;
        MarkAsUpdated();
    }

    public void SetProgress(decimal currentValue)
    {
        if (currentValue < 0)
            throw new ArgumentOutOfRangeException(nameof(currentValue));

        CurrentValue = Math.Min(currentValue, TargetValue);

        if (CurrentValue >= TargetValue)
            Status = GoalStatus.Completed;
        else if (Status == GoalStatus.NotStarted)
            Status = GoalStatus.Active;

        MarkAsUpdated();
    }

    public void AddProgress(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        SetProgress(CurrentValue + amount);
    }

    public decimal GetProgressPercentage()
    {
        return Math.Round(
            CurrentValue / TargetValue * 100,
            2);
    }

    public void SetTargetDate(DateTimeOffset? targetDateUtc)
    {
        TargetDateUtc = targetDateUtc;
        MarkAsUpdated();
    }

    public void LinkProject(Guid projectId)
    {
        ProjectId = projectId;
        MarkAsUpdated();
    }

    public void SetParentGoal(Guid parentGoalId)
    {
        ParentGoalId = parentGoalId;
        MarkAsUpdated();
    }

    public void RemoveParentGoal()
    {
        ParentGoalId = null;
        MarkAsUpdated();
    }
}
