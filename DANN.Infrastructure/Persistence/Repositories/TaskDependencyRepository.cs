using DANN.Application.Abstractions.Persistence;
using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DANN.Infrastructure.Persistence.Repositories;

public sealed class TaskDependencyRepository
    : ITaskDependencyRepository
{
    private readonly DANNDbContext _context;

    public TaskDependencyRepository(
        DANNDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ExistsAsync(
        Guid taskItemId,
        Guid dependsOnTaskItemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskDependencies
            .AnyAsync(
                x =>
                    x.TaskItemId == taskItemId &&
                    x.DependsOnTaskItemId == dependsOnTaskItemId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TaskDependency>> GetByTaskAsync(
        Guid taskItemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskDependencies
            .Where(x => x.TaskItemId == taskItemId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TaskDependency dependency,
        CancellationToken cancellationToken = default)
    {
        await _context.TaskDependencies.AddAsync(
            dependency,
            cancellationToken);
    }

    public async Task<TaskDependency?> GetAsync(
        Guid taskItemId,
        Guid dependsOnTaskItemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskDependencies
            .FirstOrDefaultAsync(
                x =>
                    x.TaskItemId == taskItemId &&
                    x.DependsOnTaskItemId == dependsOnTaskItemId,
                cancellationToken);
    }

    public void Remove(TaskDependency dependency)
    {
        _context.TaskDependencies.Remove(dependency);
    }
}
