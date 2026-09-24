namespace DANN.Domain.Enums;

public enum AIToolExecutionStatus
{
    PendingApproval = 0,
    Approved = 1,
    Running = 2,
    Succeeded = 3,
    Failed = 4,
    Rejected = 5,
    Cancelled = 6
}
