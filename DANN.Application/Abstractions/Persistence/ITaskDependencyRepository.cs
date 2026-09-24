using DANN.Domain.Entities;

namespace DANN.Application.Abstractions.Persistence;

public interface ITaskDependencyRepository
{
    Task<bool> ExistsAsync(
        Guid taskItemId,
        Guid dependsOnTaskItemId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskDependency>> GetByTaskAsync(
        Guid taskItemId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TaskDependency dependency,
        CancellationToken cancellationToken = default);

    Task<TaskDependency?> GetAsync(
        Guid taskItemId,
        Guid dependsOnTaskItemId,
        CancellationToken cancellationToken = default);

    void Remove(TaskDependency dependency);
}
