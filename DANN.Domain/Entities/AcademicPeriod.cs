using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class AcademicPeriod : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateTimeOffset StartDateUtc { get; private set; }

    public DateTimeOffset EndDateUtc { get; private set; }

    public bool IsCurrent { get; private set; }

    private AcademicPeriod()
    {
    }

    public AcademicPeriod(
        Guid ownerUserId,
        string name,
        DateTimeOffset startDateUtc,
        DateTimeOffset endDateUtc,
        bool isCurrent = false)
    {
        if (endDateUtc <= startDateUtc)
            throw new ArgumentException(
                "End date must be later than start date.");

        OwnerUserId = ownerUserId;
        Name = name.Trim();
        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;
        IsCurrent = isCurrent;
    }

    public void SetAsCurrent()
    {
        IsCurrent = true;
        MarkAsUpdated();
    }

    public void UnsetAsCurrent()
    {
        IsCurrent = false;
        MarkAsUpdated();
    }

    public void UpdateDates(
        DateTimeOffset startDateUtc,
        DateTimeOffset endDateUtc)
    {
        if (endDateUtc <= startDateUtc)
            throw new ArgumentException(
                "End date must be later than start date.");

        StartDateUtc = startDateUtc;
        EndDateUtc = endDateUtc;

        MarkAsUpdated();
    }
}
