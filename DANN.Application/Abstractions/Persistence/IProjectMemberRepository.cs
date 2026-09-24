using DANN.Domain.Entities;

namespace DANN.Application.Abstractions.Persistence;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectMember>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
}
