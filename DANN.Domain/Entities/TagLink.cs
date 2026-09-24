using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class TagLink : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid TagId { get; private set; }

    public TagTargetType TargetType { get; private set; }

    public Guid TargetId { get; private set; }

    private TagLink()
    {
    }

    public TagLink(
        Guid ownerUserId,
        Guid tagId,
        TagTargetType targetType,
        Guid targetId)
    {
        OwnerUserId = ownerUserId;
        TagId = tagId;
        TargetType = targetType;
        TargetId = targetId;
    }
}
