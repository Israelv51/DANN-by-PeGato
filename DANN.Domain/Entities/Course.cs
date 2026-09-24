using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class Course : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid AcademicPeriodId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Code { get; private set; } = string.Empty;

    public string Instructor { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    private Course()
    {
    }

    public Course(
        Guid ownerUserId,
        Guid academicPeriodId,
        string name,
        string code = "",
        string instructor = "")
    {
        OwnerUserId = ownerUserId;
        AcademicPeriodId = academicPeriodId;
        Name = name.Trim();
        Code = code.Trim();
        Instructor = instructor.Trim();
    }

    public void UpdateInformation(
        string name,
        string code,
        string instructor)
    {
        Name = name.Trim();
        Code = code.Trim();
        Instructor = instructor.Trim();

        MarkAsUpdated();
    }

    public void ChangeAcademicPeriod(Guid academicPeriodId)
    {
        AcademicPeriodId = academicPeriodId;
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
