using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.EfCore.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CQ.Extensions.ServiceCollection;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.EfCore.Core;

namespace CQ.UnitOfWork.EfCore.Configuration;
public static class UnitOfWorkEfCoreConfig
{
    #region Add Unit Of Work
    public static IServiceCollection AddUnitOfWork(
        this IServiceCollection services,
        LifeTime lifeTime)
    {
        services.AddUnitOfWork<EfCoreContext>(lifeTime);

        return services;
    }

    public static IServiceCollection AddUnitOfWork<TDbContext>(
        this IServiceCollection services,
        LifeTime lifeTime)
        where TDbContext : EfCoreContext
    {
        services.AddService<IUnitOfWork, UnitOfWorkService<TDbContext>>(lifeTime);

        return services;
    }
    #endregion

    #region Add Context
    public static IServiceCollection AddContext<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder,
        LifeTime contextLifeTime)
        where TContext : EfCoreContext
    {
        services.AddEfCoreContext<TContext>(optionsBuilder, contextLifeTime);

        return services;
    }

    public static IServiceCollection AddEfCoreContext<TContext>(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsBuilder,
        LifeTime contextLifeTime)
        where TContext : EfCoreContext
    {
        var lifeTimeDb = contextLifeTime switch
        {
            LifeTime.Transient => ServiceLifetime.Transient,
            LifeTime.Scoped => ServiceLifetime.Scoped,
            _ => ServiceLifetime.Singleton,
        };

        services
            .AddDbContext<TContext>(optionsBuilder, lifeTimeDb)
            .AddService<DbContext, TContext>(contextLifeTime)
            .AddService<EfCoreContext, TContext>(contextLifeTime)
            .AddService<IDatabaseContext, TContext>(contextLifeTime)
            ;

        return services;
    }
    #endregion

    #region Add Repository

    #region Single Context
    public static IServiceCollection AddRepository(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
    {
        services
            .AddService(typeof(IRepository<>), typeof(EfCoreRepository<>), repositoryLifeTime)
            .AddService(typeof(IEfCoreRepository<>), typeof(EfCoreRepository<>), repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddEfCoreRepository(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
    {
        services.AddRepository(repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddRepository<TEntity>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
    {
        services
            .AddService<IRepository<TEntity>, EfCoreRepository<TEntity>>(repositoryLifeTime)
            .AddService<IEfCoreRepository<TEntity>, EfCoreRepository<TEntity>>(repositoryLifeTime)
            ;

        return services;
    }

    public static IServiceCollection AddEfCoreRepository<TEntity>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
    {
        services.AddRepository<TEntity>(repositoryLifeTime);

        return services;
    }
    #endregion

    #region Specific Context
    public static IServiceCollection AddRepositoryForContext<TEntity, TContext>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TContext : EfCoreContext
    {
        services
            .AddService<IRepository<TEntity>, EfCoreRepositoryContext<TEntity, TContext>>(repositoryLifeTime)
            .AddService<IEfCoreRepository<TEntity>, EfCoreRepositoryContext<TEntity, TContext>>(repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddEfCoreRepositoryForContext<TEntity, TContext>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TContext : EfCoreContext
    {
        services.AddRepositoryForContext<TEntity, TContext>(repositoryLifeTime);

        return services;
    }
    #endregion

    #endregion

    #region Add Concrete Repository

    public static IServiceCollection AddConcreteRepository<TEntity, TConcreteRepository>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TConcreteRepository : EfCoreRepository<TEntity>
    {
        services
            .AddService<TConcreteRepository>(repositoryLifeTime)
            .AddService<IRepository<TEntity>, TConcreteRepository>(repositoryLifeTime)
            .AddService<IEfCoreRepository<TEntity>, TConcreteRepository>(repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddConcreteEfCoreRepository<TEntity, TConcreteRepository>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TConcreteRepository : EfCoreRepository<TEntity>
    {
        services.AddConcreteRepository<TEntity, TConcreteRepository>(repositoryLifeTime);

        return services;
    }
    #endregion

    #region Add Abstraction Repository

    public static IServiceCollection AddAbstractionRepository<TEntity, TRepository, TImplementation>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TRepository : class
        where TImplementation : EfCoreRepository<TEntity>, TRepository
    {
        services
            .AddService<TImplementation>(repositoryLifeTime)
            .AddService<TRepository, TImplementation>(repositoryLifeTime)
            .AddService<IRepository<TEntity>, TImplementation>(repositoryLifeTime)
            .AddService<IEfCoreRepository<TEntity>, TImplementation>(repositoryLifeTime)
            ;
        return services;
    }

    public static IServiceCollection AddEfCoreAbstractionRepository<TEntity, TRepository, TImplementation>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TRepository : class
        where TImplementation : EfCoreRepository<TEntity>, TRepository
    {
        services.AddAbstractionRepository<TEntity, TRepository, TImplementation>(repositoryLifeTime)
            ;

        return services;
    }
    #endregion
}