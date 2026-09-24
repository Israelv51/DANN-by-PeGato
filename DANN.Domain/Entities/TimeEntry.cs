using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? GoalId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset StartedAtUtc { get; private set; }

    public DateTimeOffset? EndedAtUtc { get; private set; }

    public bool IsRunning => EndedAtUtc is null;

    private TimeEntry()
    {
    }

    public TimeEntry(
        Guid ownerUserId,
        DateTimeOffset startedAtUtc,
        string description = "")
    {
        OwnerUserId = ownerUserId;
        StartedAtUtc = startedAtUtc;
        Description = description.Trim();
    }

    public void Stop(DateTimeOffset endedAtUtc)
    {
        if (EndedAtUtc is not null)
            throw new InvalidOperationException(
                "This time entry has already been stopped.");

        if (endedAtUtc <= StartedAtUtc)
            throw new ArgumentException(
                "End time must be later than start time.");

        EndedAtUtc = endedAtUtc;

        MarkAsUpdated();
    }

    public void UpdatePeriod(
        DateTimeOffset startedAtUtc,
        DateTimeOffset? endedAtUtc)
    {
        if (endedAtUtc is not null &&
            endedAtUtc <= startedAtUtc)
        {
            throw new ArgumentException(
                "End time must be later than start time.");
        }

        StartedAtUtc = startedAtUtc;
        EndedAtUtc = endedAtUtc;

        MarkAsUpdated();
    }

    public TimeSpan GetDuration()
    {
        var end = EndedAtUtc ?? DateTimeOffset.UtcNow;

        return end - StartedAtUtc;
    }

    public decimal GetDurationHours()
    {
        return Math.Round(
            (decimal)GetDuration().TotalHours,
            2);
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();
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

    public void LinkCourse(Guid courseId)
    {
        CourseId = courseId;
        MarkAsUpdated();
    }

    public void LinkGoal(Guid goalId)
    {
        GoalId = goalId;
        MarkAsUpdated();
    }
}
