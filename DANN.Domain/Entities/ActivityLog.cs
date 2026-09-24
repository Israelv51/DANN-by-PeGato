using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public ActivitySource Source { get; private set; }

    public ActivityType Type { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string EntityType { get; private set; } = string.Empty;

    public Guid? EntityId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    private ActivityLog()
    {
    }

    public ActivityLog(
        Guid ownerUserId,
        ActivitySource source,
        ActivityType type,
        string action,
        string description = "",
        string entityType = "",
        Guid? entityId = null)
    {
        OwnerUserId = ownerUserId;
        Source = source;
        Type = type;
        Action = action.Trim();
        Description = description.Trim();
        EntityType = entityType.Trim();
        EntityId = entityId;
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }
}
