using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class FileReference : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? CourseId { get; private set; }

    public Guid? TaskItemId { get; private set; }

    public Guid? ClientId { get; private set; }

    public Guid? AssignmentId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Path { get; private set; } = string.Empty;

    public string Extension { get; private set; } = string.Empty;

    public string MimeType { get; private set; } = string.Empty;

    public long SizeBytes { get; private set; }

    private FileReference()
    {
    }

    public FileReference(
        Guid ownerUserId,
        string name,
        string path,
        string extension = "",
        string mimeType = "",
        long sizeBytes = 0)
    {
        if (sizeBytes < 0)
            throw new ArgumentOutOfRangeException(nameof(sizeBytes));

        OwnerUserId = ownerUserId;
        Name = name.Trim();
        Path = path.Trim();
        Extension = extension.Trim().ToLowerInvariant();
        MimeType = mimeType.Trim().ToLowerInvariant();
        SizeBytes = sizeBytes;
    }

    public void UpdateLocation(string path)
    {
        Path = path.Trim();
        MarkAsUpdated();
    }

    public void Rename(string name)
    {
        Name = name.Trim();
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

    public void LinkAssignment(Guid assignmentId)
    {
        AssignmentId = assignmentId;
        MarkAsUpdated();
    }
}
