using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public NotificationType Type { get; private set; }

    public NotificationStatus Status { get; private set; }
        = NotificationStatus.Unread;

    public DateTimeOffset? ScheduledForUtc { get; private set; }

    public DateTimeOffset? ReadAtUtc { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public Guid? CalendarEventId { get; private set; }

    public Guid? GoalId { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid ownerUserId,
        string title,
        string message,
        NotificationType type = NotificationType.Information,
        DateTimeOffset? scheduledForUtc = null)
    {
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Message = message.Trim();
        Type = type;
        ScheduledForUtc = scheduledForUtc;
        Status = NotificationStatus.Unread;
    }

    public void MarkAsRead()
    {
        Status = NotificationStatus.Read;
        ReadAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void MarkAsUnread()
    {
        Status = NotificationStatus.Unread;
        ReadAtUtc = null;

        MarkAsUpdated();
    }

    public void Dismiss()
    {
        Status = NotificationStatus.Dismissed;

        MarkAsUpdated();
    }

    public void Schedule(DateTimeOffset scheduledForUtc)
    {
        ScheduledForUtc = scheduledForUtc;

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

    public void LinkCalendarEvent(Guid calendarEventId)
    {
        CalendarEventId = calendarEventId;
        MarkAsUpdated();
    }

    public void LinkGoal(Guid goalId)
    {
        GoalId = goalId;
        MarkAsUpdated();
    }
}
