using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class FinancialTransaction : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ProjectId { get; private set; }

    public Guid? ClientId { get; private set; }

    public Guid? PaymentId { get; private set; }

    public TransactionType Type { get; private set; }

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = "MXN";

    public string Category { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAtUtc { get; private set; }

    private FinancialTransaction()
    {
    }

    public FinancialTransaction(
        Guid ownerUserId,
        TransactionType type,
        decimal amount,
        string category,
        string description = "",
        string currency = "MXN",
        DateTimeOffset? occurredAtUtc = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Transaction amount must be greater than zero.");

        OwnerUserId = ownerUserId;
        Type = type;
        Amount = amount;
        Category = category.Trim();
        Description = description.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        OccurredAtUtc = occurredAtUtc ?? DateTimeOffset.UtcNow;
    }

    public void UpdateDetails(
        TransactionType type,
        decimal amount,
        string category,
        string description,
        string currency)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Transaction amount must be greater than zero.");

        Type = type;
        Amount = amount;
        Category = category.Trim();
        Description = description.Trim();
        Currency = currency.Trim().ToUpperInvariant();

        MarkAsUpdated();
    }

    public void ChangeDate(DateTimeOffset occurredAtUtc)
    {
        OccurredAtUtc = occurredAtUtc;
        MarkAsUpdated();
    }

    public void LinkProject(Guid projectId)
    {
        ProjectId = projectId;
        MarkAsUpdated();
    }

    public void LinkClient(Guid clientId)
    {
        ClientId = clientId;
        MarkAsUpdated();
    }

    public void LinkPayment(Guid paymentId)
    {
        PaymentId = paymentId;
        MarkAsUpdated();
    }
}
