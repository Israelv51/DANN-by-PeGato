using DANN.Application.Abstractions.Persistence;
using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DANN.Infrastructure.Persistence.Repositories;

public sealed class TaskItemRepository : ITaskItemRepository
{
    private readonly DANNDbContext _context;

    public TaskItemRepository(DANNDbContext context)
    {
        _context = context;
    }

    public async Task<TaskItem?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByOwnerAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderBy(x => x.DueDateUtc)
            .ThenByDescending(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.DueDateUtc)
            .ThenByDescending(x => x.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetChildrenAsync(
        Guid parentTaskItemId,
        CancellationToken cancellationToken = default)
    {
        return await _context.TaskItems
            .Where(x =>
                x.ParentTaskItemId == parentTaskItemId)
            .OrderBy(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        TaskItem taskItem,
        CancellationToken cancellationToken = default)
    {
        await _context.TaskItems.AddAsync(
            taskItem,
            cancellationToken);
    }
}
