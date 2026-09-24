using DANN.Application.Abstractions.Persistence;
using DANN.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DANN.Infrastructure.Persistence.Repositories;

public sealed class ProjectMemberRepository
    : IProjectMemberRepository
{
    private readonly DANNDbContext _context;

    public ProjectMemberRepository(
        DANNDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProjectMembers
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<ProjectMember>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProjectMembers
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.DisplayName)
            .ToListAsync(cancellationToken);
    }
}
