using DANN.Application.DTOs;
using DANN.Domain.Entities;

namespace DANN.Application.Common.Mappings;

public static class ProjectMapper
{
    public static ProjectDto ToDto(Project project)
    {
        return new ProjectDto(
            project.Id,
            project.OwnerUserId,
            project.ClientId,
            project.Name,
            project.Description,
            project.Type,
            project.Status,
            project.TargetDateUtc,
            project.AgreedAmount,
            project.Currency,
            project.IsArchived,
            project.ArchivedAtUtc,
            project.CreatedAtUtc,
            project.UpdatedAtUtc);
    }
}
