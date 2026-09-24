using DANN.Domain.Common;
using DANN.Domain.Enums;

namespace DANN.Domain.Entities;

public class AutomationExecution : BaseEntity
{
    public Guid OwnerUserId { get; private set; }

    public Guid AutomationRuleId { get; private set; }

    public AutomationExecutionStatus Status { get; private set; }
        = AutomationExecutionStatus.Pending;

    public DateTimeOffset? StartedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public string InputData { get; private set; } = string.Empty;

    public string OutputData { get; private set; } = string.Empty;

    public string ErrorMessage { get; private set; } = string.Empty;

    private AutomationExecution()
    {
    }

    public AutomationExecution(
        Guid ownerUserId,
        Guid automationRuleId,
        string inputData = "")
    {
        OwnerUserId = ownerUserId;
        AutomationRuleId = automationRuleId;
        InputData = inputData.Trim();
        Status = AutomationExecutionStatus.Pending;
    }

    public void Start()
    {
        Status = AutomationExecutionStatus.Running;
        StartedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Succeed(string outputData = "")
    {
        Status = AutomationExecutionStatus.Succeeded;
        OutputData = outputData.Trim();
        ErrorMessage = string.Empty;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Fail(string errorMessage)
    {
        Status = AutomationExecutionStatus.Failed;
        ErrorMessage = errorMessage.Trim();
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public void Cancel()
    {
        Status = AutomationExecutionStatus.Cancelled;
        CompletedAtUtc = DateTimeOffset.UtcNow;

        MarkAsUpdated();
    }

    public TimeSpan? GetDuration()
    {
        if (StartedAtUtc is null)
            return null;

        var end = CompletedAtUtc ?? DateTimeOffset.UtcNow;

        return end - StartedAtUtc.Value;
    }
}
