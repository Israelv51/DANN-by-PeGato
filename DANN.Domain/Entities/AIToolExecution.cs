using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class AIToolExecution : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid? ConversationId { get; private set; }

    public Guid? AIMessageId { get; private set; }

    public string ToolName { get; private set; } = string.Empty;

    public string ActionName { get; private set; } = string.Empty;

    public string InputData { get; private set; } = string.Empty;

    public string OutputData { get; private set; } = string.Empty;

    public string ErrorMessage { get; private set; } = string.Empty;

    public AIToolRiskLevel RiskLevel { get; private set; }

    public AIToolExecutionStatus Status { get; private set; }

    public DateTimeOffset? ApprovedAtUtc { get; private set; }

    public DateTimeOffset? StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    private AIToolExecution()
    {
    }

    public AIToolExecution(
        Guid ownerUserId,
        string toolName,
        string actionName,
        AIToolRiskLevel riskLevel,
        string inputData = "",
        Guid? conversationId = null,
        Guid? aiMessageId = null)
    {
        if (string.IsNullOrWhiteSpace(toolName))
            throw new ArgumentException(
                "Tool name cannot be empty.",
                nameof(toolName));

        if (string.IsNullOrWhiteSpace(actionName))
            throw new ArgumentException(
                "Action name cannot be empty.",
                nameof(actionName));

        OwnerUserId = ownerUserId;
        ToolName = toolName.Trim();
        ActionName = actionName.Trim();
        RiskLevel = riskLevel;
        InputData = inputData.Trim();
        ConversationId = conversationId;
        AIMessageId = aiMessageId;

        Status = riskLevel == AIToolRiskLevel.Sensitive
            ? AIToolExecutionStatus.PendingApproval
            : AIToolExecutionStatus.Approved;
    }

    public bool RequiresApproval()
    {
        return RiskLevel == AIToolRiskLevel.Sensitive;
    }

    public void Approve()
    {
        if (Status != AIToolExecutionStatus.PendingApproval)
            throw new InvalidOperationException(
                "This execution is not waiting for approval.");

        Status = AIToolExecutionStatus.Approved;
        ApprovedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Reject()
    {
        if (Status != AIToolExecutionStatus.PendingApproval)
            throw new InvalidOperationException(
                "This execution is not waiting for approval.");

        Status = AIToolExecutionStatus.Rejected;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Start()
    {
        if (Status != AIToolExecutionStatus.Approved)
            throw new InvalidOperationException(
                "The execution must be approved before it can start.");

        Status = AIToolExecutionStatus.Running;
        StartedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Succeed(string outputData = "")
    {
        if (Status != AIToolExecutionStatus.Running)
            throw new InvalidOperationException(
                "Only a running execution can succeed.");

        Status = AIToolExecutionStatus.Succeeded;
        OutputData = outputData.Trim();
        ErrorMessage = string.Empty;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Fail(string errorMessage)
    {
        if (Status != AIToolExecutionStatus.Running)
            throw new InvalidOperationException(
                "Only a running execution can fail.");

        Status = AIToolExecutionStatus.Failed;
        ErrorMessage = errorMessage.Trim();
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Cancel()
    {
        if (Status == AIToolExecutionStatus.Succeeded ||
            Status == AIToolExecutionStatus.Failed ||
            Status == AIToolExecutionStatus.Rejected)
        {
            throw new InvalidOperationException(
                "A finished execution cannot be cancelled.");
        }

        Status = AIToolExecutionStatus.Cancelled;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }
}
