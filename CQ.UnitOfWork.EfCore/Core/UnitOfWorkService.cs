using CQ.UnitOfWork.Abstractions;

namespace CQ.UnitOfWork.EfCore.Core;

public class UnitOfWorkService<TDbContext>(TDbContext context)
    : IUnitOfWork
    where TDbContext : EfCoreContext
{
    public async Task CommitChangesAsync()
    {
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}
