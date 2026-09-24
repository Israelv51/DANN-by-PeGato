using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class AIConversation : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? ClientId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public bool IsArchived { get; private set; }

    public DateTimeOffset? LastMessageAtUtc { get; private set; }

    private AIConversation()
    {
    }

    public AIConversation(
        Guid ownerUserId,
        string title = "Nueva conversación")
    {
        OwnerUserId = ownerUserId;
        Title = title.Trim();
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Conversation title cannot be empty.",
                nameof(title));

        Title = title.Trim();
        MarkAsUpdated();
    }

    public void RegisterMessage()
    {
        LastMessageAtUtc = DateTimeOffset.UtcNow;
        MarkAsUpdated();
    }

    public void Archive()
    {
        IsArchived = true;
        MarkAsUpdated();
    }

    public void Restore()
    {
        IsArchived = false;
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
}
