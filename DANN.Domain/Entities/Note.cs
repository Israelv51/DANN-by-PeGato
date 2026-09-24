using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class Note : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public Guid? ClientId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public bool IsPinned { get; private set; }

    private Note()
    {
    }

    public Note(
        Guid ownerUserId,
        string title,
        string content = "")
    {
        OwnerUserId = ownerUserId;
        Title = title.Trim();
        Content = content.Trim();
    }

    public void Update(string title, string content)
    {
        Title = title.Trim();
        Content = content.Trim();

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

    public void LinkClient(Guid clientId)
    {
        ClientId = clientId;
        MarkAsUpdated();
    }
}
