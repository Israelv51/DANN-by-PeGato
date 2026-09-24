using DANN.Application.DTOs;
using DANN.Domain.Entities;

namespace DANN.Application.Common.Mappings;

public static class TaskItemMapper
{
    public static TaskItemDto ToDto(TaskItem taskItem)
    {
        return new TaskItemDto(
            taskItem.Id,
            taskItem.OwnerUserId,
            taskItem.ProjectId,
            taskItem.ParentTaskItemId,
            taskItem.AssigneeProjectMemberId,
            taskItem.Title,
            taskItem.Description,
            taskItem.Status,
            taskItem.Priority,
            taskItem.DueDateUtc,
            taskItem.CompletedAtUtc,
            taskItem.CreatedAtUtc,
            taskItem.UpdatedAtUtc);
    }
}
