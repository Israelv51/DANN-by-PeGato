using DANN.Domain.Common;

namespace DANN.Domain.Entities;

public class Client : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Company { get; private set; } = string.Empty;

    public string Notes { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    private Client()
    {
    }

    public Client(
        Guid ownerUserId,
        string name,
        string email = "",
        string phone = "",
        string company = "")
    {
        OwnerUserId = ownerUserId;
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Company = company.Trim();
    }

    public void UpdateContact(
        string name,
        string email,
        string phone,
        string company)
    {
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Phone = phone.Trim();
        Company = company.Trim();

        MarkAsUpdated();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes.Trim();
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
