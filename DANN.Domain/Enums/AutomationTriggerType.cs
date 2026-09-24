namespace DANN.Domain.Enums;

public enum AutomationTriggerType
{
    Manual = 0,
    Scheduled = 1,
    TaskDueSoon = 2,
    TaskOverdue = 3,
    CalendarEventStartingSoon = 4,
    GoalDeadlineApproaching = 5,
    ProjectInactive = 6
}
