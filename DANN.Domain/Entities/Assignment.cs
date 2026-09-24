using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid CourseId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public AssignmentType Type { get; private set; }

    public AssignmentStatus Status { get; private set; }
        = AssignmentStatus.Pending;

    public DateTimeOffset? DueDateUtc { get; private set; }

    public DateTimeOffset? SubmittedAtUtc { get; private set; }

    public decimal WeightPercentage { get; private set; }

    public decimal? Score { get; private set; }

    public decimal? MaxScore { get; private set; }

    private Assignment()
    {
    }

    public Assignment(
        Guid ownerUserId,
        Guid courseId,
        string title,
        AssignmentType type,
        string description = "",
        DateTimeOffset? dueDateUtc = null,
        decimal weightPercentage = 0)
    {
        if (weightPercentage < 0 || weightPercentage > 100)
            throw new ArgumentOutOfRangeException(
                nameof(weightPercentage),
                "Weight must be between 0 and 100.");

        OwnerUserId = ownerUserId;
        CourseId = courseId;
        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        DueDateUtc = dueDateUtc;
        WeightPercentage = weightPercentage;
        Status = AssignmentStatus.Pending;
    }

    public void Start()
    {
        Status = AssignmentStatus.InProgress;
        MarkAsUpdated();
    }

    public void Submit()
    {
        Status = AssignmentStatus.Submitted;
        SubmittedAtUtc = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Grade(decimal score, decimal maxScore)
    {
        if (maxScore <= 0)
            throw new ArgumentException(
                "Max score must be greater than zero.",
                nameof(maxScore));

        if (score < 0 || score > maxScore)
            throw new ArgumentOutOfRangeException(nameof(score));

        Score = score;
        MaxScore = maxScore;
        Status = AssignmentStatus.Graded;

        MarkAsUpdated();
    }

    public void SetWeight(decimal weightPercentage)
    {
        if (weightPercentage < 0 || weightPercentage > 100)
            throw new ArgumentOutOfRangeException(
                nameof(weightPercentage),
                "Weight must be between 0 and 100.");

        WeightPercentage = weightPercentage;
        MarkAsUpdated();
    }

    public decimal? GetPercentage()
    {
        if (Score is null || MaxScore is null || MaxScore == 0)
            return null;

        return Math.Round(
            Score.Value / MaxScore.Value * 100,
            2);
    }

    public decimal? GetWeightedContribution()
    {
        var percentage = GetPercentage();

        if (percentage is null)
            return null;

        return Math.Round(
            percentage.Value * WeightPercentage / 100,
            2);
    }

    public void LinkTask(Guid taskItemId)
    {
        TaskItemId = taskItemId;
        MarkAsUpdated();
    }

    public void Reschedule(DateTimeOffset? dueDateUtc)
    {
        DueDateUtc = dueDateUtc;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = AssignmentStatus.Cancelled;
        MarkAsUpdated();
    }
}
