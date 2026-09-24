using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class Tag : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    private Tag()
    {
    }

    public Tag(
        Guid ownerUserId,
        string name,
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Tag name cannot be empty.",
                nameof(name));

        OwnerUserId = ownerUserId;
        Name = name.Trim();
        Description = description.Trim();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Tag name cannot be empty.",
                nameof(name));

        Name = name.Trim();

        MarkAsUpdated();
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();

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
}
