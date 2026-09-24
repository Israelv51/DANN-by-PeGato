using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class AIMessage : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid ConversationId { get; private set; }

    public AIMessageRole Role { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public string ModelName { get; private set; } = string.Empty;

    public int? InputTokens { get; private set; }

    public int? OutputTokens { get; private set; }

    private AIMessage()
    {
    }

    public AIMessage(
        Guid ownerUserId,
        Guid conversationId,
        AIMessageRole role,
        string content,
        string modelName = "")
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Message content cannot be empty.",
                nameof(content));

        OwnerUserId = ownerUserId;
        ConversationId = conversationId;
        Role = role;
        Content = content.Trim();
        ModelName = modelName.Trim();
    }

    public void RegisterTokenUsage(
        int? inputTokens,
        int? outputTokens)
    {
        if (inputTokens is < 0)
            throw new ArgumentOutOfRangeException(nameof(inputTokens));

        if (outputTokens is < 0)
            throw new ArgumentOutOfRangeException(nameof(outputTokens));

        InputTokens = inputTokens;
        OutputTokens = outputTokens;

        MarkAsUpdated();
    }
}
