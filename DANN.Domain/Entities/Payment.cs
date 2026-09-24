using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid ProjectId { get; private set; }

    public Guid? ClientId { get; private set; }

    public string Concept { get; private set; } = string.Empty;

    public decimal Amount { get; private set; }

    public string Currency { get; private set; } = "MXN";

    public PaymentStatus Status { get; private set; }
        = PaymentStatus.Pending;

    public DateTimeOffset? DueDateUtc { get; private set; }

    public DateTimeOffset? PaidAtUtc { get; private set; }

    public string Notes { get; private set; } = string.Empty;

    private Payment()
    {
    }

    public Payment(
        Guid ownerUserId,
        Guid projectId,
        decimal amount,
        string concept,
        string currency = "MXN",
        Guid? clientId = null,
        DateTimeOffset? dueDateUtc = null)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Payment amount must be greater than zero.");

        OwnerUserId = ownerUserId;
        ProjectId = projectId;
        ClientId = clientId;
        Amount = amount;
        Concept = concept.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        DueDateUtc = dueDateUtc;
        Status = PaymentStatus.Pending;
    }

    public void MarkAsPaid(DateTimeOffset paidAtUtc)
    {
        Status = PaymentStatus.Paid;
        PaidAtUtc = paidAtUtc;

        MarkAsUpdated();
    }

    public void MarkAsOverdue()
    {
        if (Status == PaymentStatus.Paid)
            throw new InvalidOperationException(
                "A paid payment cannot be marked as overdue.");

        Status = PaymentStatus.Overdue;
        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = PaymentStatus.Cancelled;
        MarkAsUpdated();
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Paid)
            throw new InvalidOperationException(
                "Only paid payments can be refunded.");

        Status = PaymentStatus.Refunded;
        MarkAsUpdated();
    }

    public void Reschedule(DateTimeOffset? dueDateUtc)
    {
        DueDateUtc = dueDateUtc;

        if (Status == PaymentStatus.Overdue)
            Status = PaymentStatus.Pending;

        MarkAsUpdated();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes.Trim();
        MarkAsUpdated();
    }
}
