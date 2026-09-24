using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class ProjectMember : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid ProjectId { get; private set; }

    public Guid? UserId { get; private set; }

    public string DisplayName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Role { get; private set; } = string.Empty;

    public string Responsibilities { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public DateTimeOffset JoinedAtUtc { get; private set; }

    public DateTimeOffset? LeftAtUtc { get; private set; }

    private ProjectMember()
    {
    }

    public ProjectMember(
        Guid ownerUserId,
        Guid projectId,
        string displayName,
        string role,
        string email = "",
        string responsibilities = "",
        Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Display name cannot be empty.",
                nameof(displayName));

        OwnerUserId = ownerUserId;
        ProjectId = projectId;
        UserId = userId;
        DisplayName = displayName.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = role.Trim();
        Responsibilities = responsibilities.Trim();
        JoinedAtUtc = DateTimeOffset.UtcNow;
    }

    public void UpdateInformation(
        string displayName,
        string role,
        string email,
        string responsibilities)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException(
                "Display name cannot be empty.",
                nameof(displayName));

        DisplayName = displayName.Trim();
        Role = role.Trim();
        Email = email.Trim().ToLowerInvariant();
        Responsibilities = responsibilities.Trim();

        MarkAsUpdated();
    }

    public void LinkUser(Guid userId)
    {
        UserId = userId;
        MarkAsUpdated();
    }

    public void RemoveUserLink()
    {
        UserId = null;
        MarkAsUpdated();
    }

    public void LeaveProject()
    {
        if (!IsActive)
            return;

        IsActive = false;
        LeftAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Reactivate()
    {
        IsActive = true;
        LeftAtUtc = null;

        MarkAsUpdated();
    }
}
