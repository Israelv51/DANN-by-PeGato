using DANN.Application.Abstractions.Persistence;
using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DANN.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly DANNDbContext _context;

    public ProjectRepository(DANNDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> GetByOwnerAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(
        Guid ownerUserId,
        string name,
        Guid? excludeProjectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Projects
            .Where(x =>
                x.OwnerUserId == ownerUserId &&
                x.Name == name);

        if (excludeProjectId.HasValue)
        {
            query = query.Where(
                x => x.Id != excludeProjectId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(
            project,
            cancellationToken);
    }
}
