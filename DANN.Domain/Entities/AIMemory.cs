using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class AIMemory : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? ClientId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public AIMemoryType Type { get; private set; }

    public int Importance { get; private set; } = 50;

    public bool IsActive { get; private set; } = true;

    public bool IsPinned { get; private set; }

    public DateTimeOffset? LastUsedAtUtc { get; private set; }

    private AIMemory()
    {
    }

    public AIMemory(
        Guid ownerUserId,
        string title,
        string content,
        AIMemoryType type,
        int importance = 50)
    {
        if (importance < 0 || importance > 100)
            throw new ArgumentOutOfRangeException(
                nameof(importance),
                "Importance must be between 0 and 100.");

        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Content = content.Trim();
        Type = type;
        Importance = importance;
    }

    public void Update(
        string title,
        string content,
        AIMemoryType type)
    {
        Title = title.Trim();
        Content = content.Trim();
        Type = type;

        MarkAsUpdated();
    }

    public void ChangeImportance(int importance)
    {
        if (importance < 0 || importance > 100)
            throw new ArgumentOutOfRangeException(
                nameof(importance),
                "Importance must be between 0 and 100.");

        Importance = importance;
        MarkAsUpdated();
    }

    public void MarkAsUsed()
    {
        LastUsedAtUtc = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Pin()
    {
        IsPinned = true;
        MarkAsUpdated();
    }

    public void Unpin()
    {
        IsPinned = false;
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
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

    public void LinkClient(Guid clientId)
    {
        ClientId = clientId;
        MarkAsUpdated();
    }

    public void RemoveProjectLink()
    {
        ProjectId = null;
        MarkAsUpdated();
    }

    public void RemoveCourseLink()
    {
        CourseId = null;
        MarkAsUpdated();
    }

    public void RemoveClientLink()
    {
        ClientId = null;
        MarkAsUpdated();
    }
}
