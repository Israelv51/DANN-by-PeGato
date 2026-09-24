using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Project : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ClientId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public ProjectType Type { get; private set; }
        = ProjectType.Personal;

    public ProjectStatus Status { get; private set; }
        = ProjectStatus.Planning;

    public DateTimeOffset? TargetDateUtc { get; private set; }

    public decimal? AgreedAmount { get; private set; }

    public string Currency { get; private set; } = "MXN";

    public bool IsArchived { get; private set; }

    public DateTimeOffset? ArchivedAtUtc { get; private set; }

    private Project()
    {
    }

    public Project(
        Guid ownerUserId,
        string name,
        string description = "",
        DateTimeOffset? targetDateUtc = null,
        Guid? clientId = null,
        ProjectType type = ProjectType.Personal)
    {
        OwnerUserId = ownerUserId;
        ClientId = clientId;
        Name = name.Trim();
        Description = description.Trim();
        TargetDateUtc = targetDateUtc;
        Type = type;
        Status = ProjectStatus.Planning;
    }

    public void Rename(string name)
    {
        Name = name.Trim();
        MarkAsUpdated();
    }

    public void UpdateDescription(string description)
    {
        Description = description.Trim();
        MarkAsUpdated();
    }

    public void ChangeType(ProjectType type)
    {
        Type = type;
        MarkAsUpdated();
    }

    public void SetTargetDate(DateTimeOffset? targetDateUtc)
    {
        TargetDateUtc = targetDateUtc;
        MarkAsUpdated();
    }

    public void LinkClient(Guid clientId)
    {
        ClientId = clientId;
        MarkAsUpdated();
    }

    public void RemoveClient()
    {
        ClientId = null;
        MarkAsUpdated();
    }

    public void SetFinancialTerms(
        decimal agreedAmount,
        string currency = "MXN")
    {
        if (agreedAmount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(agreedAmount),
                "Agreed amount must be greater than zero.");

        AgreedAmount = agreedAmount;
        Currency = currency.Trim().ToUpperInvariant();

        MarkAsUpdated();
    }

    public void ClearFinancialTerms()
    {
        AgreedAmount = null;
        Currency = "MXN";

        MarkAsUpdated();
    }

    public void Archive()
    {
        if (IsArchived)
            return;

        IsArchived = true;
        ArchivedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Restore()
    {
        if (!IsArchived)
            return;

        IsArchived = false;
        ArchivedAtUtc = null;

        MarkAsUpdated();
    }

    public void Activate()
    {
        Status = ProjectStatus.Active;
        MarkAsUpdated();
    }

    public void Pause()
    {
        Status = ProjectStatus.OnHold;
        MarkAsUpdated();
    }

    public void Complete()
    {
        Status = ProjectStatus.Completed;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = ProjectStatus.Cancelled;
        MarkAsUpdated();
    }
}
