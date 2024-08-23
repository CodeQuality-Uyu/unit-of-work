using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.Core;
internal class UnitOfWorkService(IServiceProvider _services)
    : IUnitOfWork
{
    private IDatabaseContext _unitContext;

    public IRepository<TEntity> GetEntityRepository<TEntity>() where TEntity : class
    {
        var entityRepository = _services.GetRequiredService<IRepository<TEntity>>();

        return entityRepository;
    }

    public IRepository<TEntity> GetUnitRepository<TEntity, TContext>()
        where TEntity : class
        where TContext : IDatabaseContext
    {

        if (Guard.IsNull(_unitContext))
        {
            var context = _services.GetRequiredService<TContext>();

            _unitContext = context;
        }

        var repository = _services.GetRequiredService<IUnitRepository<TEntity>>();

        repository.SetContext(_unitContext);

        return repository;
    }

    public IRepository<TEntity> GetUnitRepository<TEntity>()
        where TEntity : class
    {
        if (Guard.IsNull(_unitContext))
        {
            var context = _services.GetRequiredService<IDatabaseContext>();

            _unitContext = context;
        }

        var repository = _services.GetRequiredService<IUnitRepository<TEntity>>();

        repository.SetContext(_unitContext);

        return repository;
    }

    public async Task CommitChangesAsync()
    {
        if (_unitContext == null)
        {
            throw new InvalidOperationException($"Unit context not setted");
        }

        //await _unitContext.SaveChangesAsync().ConfigureAwait(false);
    }

    public TRepository GetRepository<TRepository>() where TRepository : class
    {
        var repository = _services.GetService<TRepository>();

        if (Guard.IsNull(repository))
        {
            throw new ArgumentException($"Repository {typeof(TRepository).Name} not loaded");
        }

        return repository;
    }
}
