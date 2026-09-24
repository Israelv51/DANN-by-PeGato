using DANN.Domain.Entities;

namespace DANN.Application.Abstractions.Persistence;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByOwnerAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetChildrenAsync(
        Guid parentTaskItemId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TaskItem taskItem,
        CancellationToken cancellationToken = default);
}
