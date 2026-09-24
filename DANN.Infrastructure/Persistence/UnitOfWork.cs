using DANN.Application.Abstractions.Persistence;

namespace DANN.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DANNDbContext _context;

    public UnitOfWork(DANNDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }
}
