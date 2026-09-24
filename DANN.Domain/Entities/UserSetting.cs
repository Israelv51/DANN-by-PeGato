using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class UserSetting : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public string Key { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public SettingValueType ValueType { get; private set; }
        = SettingValueType.String;

    public string Category { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    private UserSetting()
    {
    }

    public UserSetting(
        Guid ownerUserId,
        string key,
        string value,
        SettingValueType valueType = SettingValueType.String,
        string category = "",
        string description = "")
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException(
                "Setting key cannot be empty.",
                nameof(key));

        OwnerUserId = ownerUserId;
        Key = key.Trim();
        Value = value.Trim();
        ValueType = valueType;
        Category = category.Trim();
        Description = description.Trim();
    }

    public void UpdateValue(
        string value,
        SettingValueType valueType)
    {
        Value = value.Trim();
        ValueType = valueType;

        MarkAsUpdated();
    }

    public void UpdateMetadata(
        string category,
        string description)
    {
        Category = category.Trim();
        Description = description.Trim();

        MarkAsUpdated();
    }
}
