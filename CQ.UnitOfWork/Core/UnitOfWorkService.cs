using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.Core;

internal class UnitOfWorkService(
    IServiceProvider _services,
    IDatabaseContext _unitContext)
    : IUnitOfWork
{
    public IRepository<TEntity> GetEntityRepository<TEntity>()
        where TEntity : class
    {
        var entityRepository = _services.GetRequiredService<IUnitRepository<TEntity>>();

        entityRepository.SetContext(_unitContext);

        return entityRepository;
    }

    public async Task CommitChangesAsync()
    {
        await _unitContext
            .SaveChangesAsync()
            .ConfigureAwait(false);
    }

    public TRepository GetRepository<TRepository, TEntity>()
        where TRepository : class
        where TEntity : class
    {
        var unitRepository = _services.GetRequiredService<IUnitRepository<TEntity>>();

        unitRepository.SetContext(_unitContext);

        var repository = (TRepository)unitRepository;

        Guard.ThrowIsNull(repository, "repository");

        return repository;
    }
}
