using DANN.Domain.Entities;

namespace DANN.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> GetByOwnerAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        Guid ownerUserId,
        string name,
        Guid? excludeProjectId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default);
}
