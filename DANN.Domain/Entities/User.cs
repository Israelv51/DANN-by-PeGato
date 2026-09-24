using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    private User()
    {
    }

    public User(string firstName, string lastName, string email)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public string GetFullName()
    {
        return $"{FirstName} {LastName}".Trim();
    }

    public void UpdateProfile(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        MarkAsUpdated();
    }

    public void ChangeEmail(string email)
    {
        Email = email.Trim().ToLowerInvariant();

        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;

        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;

        MarkAsUpdated();
    }
}
