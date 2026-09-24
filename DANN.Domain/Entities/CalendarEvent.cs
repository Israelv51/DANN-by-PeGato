using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class CalendarEvent : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string Location { get; private set; } = string.Empty;

    public CalendarEventType Type { get; private set; }

    public DateTimeOffset StartAtUtc { get; private set; }

    public DateTimeOffset EndAtUtc { get; private set; }

    public bool IsAllDay { get; private set; }

    public bool IsCancelled { get; private set; }

    private CalendarEvent()
    {
    }

    public CalendarEvent(
        Guid ownerUserId,
        string title,
        CalendarEventType type,
        DateTimeOffset startAtUtc,
        DateTimeOffset endAtUtc,
        string description = "",
        string location = "",
        bool isAllDay = false)
    {
        if (endAtUtc <= startAtUtc)
            throw new ArgumentException(
                "End date must be later than start date.");

        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Description = description.Trim();
        Location = location.Trim();
        Type = type;
        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;
        IsAllDay = isAllDay;
    }

    public void UpdateDetails(
        string title,
        string description,
        string location,
        CalendarEventType type)
    {
        Title = title.Trim();
        Description = description.Trim();
        Location = location.Trim();
        Type = type;

        MarkAsUpdated();
    }

    public void Reschedule(
        DateTimeOffset startAtUtc,
        DateTimeOffset endAtUtc)
    {
        if (endAtUtc <= startAtUtc)
            throw new ArgumentException(
                "End date must be later than start date.");

        StartAtUtc = startAtUtc;
        EndAtUtc = endAtUtc;

        MarkAsUpdated();
    }

    public void SetAllDay(bool isAllDay)
    {
        IsAllDay = isAllDay;
        MarkAsUpdated();
    }

    public void LinkProject(Guid projectId)
    {
        ProjectId = projectId;
        MarkAsUpdated();
    }

    public void LinkCourse(Guid courseId)
    {
        CourseId = courseId;
        MarkAsUpdated();
    }

    public void LinkTask(Guid taskItemId)
    {
        TaskItemId = taskItemId;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        IsCancelled = true;
        MarkAsUpdated();
    }

    public void Restore()
    {
        IsCancelled = false;
        MarkAsUpdated();
    }
}
