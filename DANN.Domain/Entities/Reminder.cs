using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Reminder : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? CalendarEventId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public DateTimeOffset RemindAtUtc { get; private set; }

    public ReminderStatus Status { get; private set; }
        = ReminderStatus.Pending;

    public DateTimeOffset? TriggeredAtUtc { get; private set; }

    private Reminder()
    {
    }

    public Reminder(
        Guid ownerUserId,
        string title,
        DateTimeOffset remindAtUtc,
        string message = "")
    {
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Message = message.Trim();
        RemindAtUtc = remindAtUtc;
        Status = ReminderStatus.Pending;
    }

    public void LinkCalendarEvent(Guid calendarEventId)
    {
        CalendarEventId = calendarEventId;
        MarkAsUpdated();
    }

    public void LinkTask(Guid taskItemId)
    {
        TaskItemId = taskItemId;
        MarkAsUpdated();
    }

    public void Reschedule(DateTimeOffset remindAtUtc)
    {
        RemindAtUtc = remindAtUtc;
        Status = ReminderStatus.Pending;
        MarkAsUpdated();
    }

    public void Trigger()
    {
        Status = ReminderStatus.Triggered;
        TriggeredAtUtc = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Snooze(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(
                nameof(duration),
                "Snooze duration must be greater than zero.");

        RemindAtUtc = DateTimeOffset.UtcNow.Add(duration);
        Status = ReminderStatus.Snoozed;

        MarkAsUpdated();
    }

    public void Complete()
    {
        Status = ReminderStatus.Completed;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = ReminderStatus.Cancelled;
        MarkAsUpdated();
    }
}
