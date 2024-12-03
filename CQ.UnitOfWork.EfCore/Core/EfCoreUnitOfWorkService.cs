using CQ.UnitOfWork.Abstractions;

namespace CQ.UnitOfWork.EfCore.Core;

public class EfCoreUnitOfWorkService<TDbContext>(TDbContext _context)
    : IUnitOfWork
    where TDbContext : EfCoreContext
{
    protected TDbContext ConcreteContext { get; } = _context;
    
    public async Task CommitChangesAsync()
    {
        await ConcreteContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
