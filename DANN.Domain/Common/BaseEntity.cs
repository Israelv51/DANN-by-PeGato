namespace DANN.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAtUtc { get; protected set; }
        = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAtUtc { get; protected set; }

    protected void MarkAsUpdated()
    {
        UpdatedAtUtc = DateTimeOffset.UtcNow;
    }
}
