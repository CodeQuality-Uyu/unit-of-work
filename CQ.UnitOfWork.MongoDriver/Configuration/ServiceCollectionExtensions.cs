using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.MongoDriver.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using CQ.Utility;
using CQ.Extensions.ServiceCollection;
using CQ.UnitOfWork.Abstractions.Repositories;
using CQ.UnitOfWork.MongoDriver.Core;

namespace CQ.UnitOfWork.MongoDriver.Configuration;
public static class ServiceCollectionExtensions
{
    #region Add Context

    public static IServiceCollection AddContext<TContext>(
        this IServiceCollection services,
        MongoClient mongoClient,
        LifeTime contextLifeTime)
        where TContext : MongoContext
    {
        services.AddMongoContext<TContext>(mongoClient, contextLifeTime);

        return services;
    }

    public static IServiceCollection AddMongoContext<TContext>(
        this IServiceCollection services,
        MongoClient mongoClient,
        LifeTime contextLifeTime)
        where TContext : MongoContext
    {
        TContext ContextFactory(IServiceProvider serviceProvider) =>
            (TContext)Activator.CreateInstance(typeof(TContext), mongoClient);

        services
            .AddService<TContext>(ContextFactory, contextLifeTime)
            .AddService<MongoContext, TContext>(contextLifeTime)
            .AddService<IDatabaseContext, TContext>(contextLifeTime);

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
           .AddService(typeof(IRepository<>), typeof(MongoDriverRepository<>), repositoryLifeTime)
           .AddService(typeof(IMongoDriverRepository<>), typeof(MongoDriverRepository<>), repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddMongoDriverRepository(
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
            .AddService<IRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryLifeTime)
            .AddService<IMongoDriverRepository<TEntity>, MongoDriverRepository<TEntity>>(repositoryLifeTime)
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
        where TContext : MongoContext
    {
        services
            .AddService<IRepository<TEntity>, MongoDriverRepositoryContext<TEntity, TContext>>(repositoryLifeTime)
            .AddService<IMongoDriverRepository<TEntity>, MongoDriverRepositoryContext<TEntity, TContext>>(repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddMongoRepositoryForContext<TEntity, TContext>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TContext : MongoContext
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
        where TConcreteRepository : MongoDriverRepository<TEntity>
    {
        services
            .AddService<TConcreteRepository>(repositoryLifeTime)
            .AddService<IRepository<TEntity>, TConcreteRepository>(repositoryLifeTime)
            .AddService<IMongoDriverRepository<TEntity>, TConcreteRepository>(repositoryLifeTime);

        return services;
    }

    public static IServiceCollection AddConcreteEfCoreRepository<TEntity, TConcreteRepository>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TConcreteRepository : MongoDriverRepository<TEntity>
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
    where TImplementation : MongoDriverRepository<TEntity>, TRepository
    {
        services
            .AddService<TImplementation>(repositoryLifeTime)
            .AddService<TRepository, TImplementation>(repositoryLifeTime)
            .AddService<IRepository<TEntity>, TImplementation>(repositoryLifeTime)
            .AddService<IMongoDriverRepository<TEntity>, TImplementation>(repositoryLifeTime)
            ;
        return services;
    }

    public static IServiceCollection AddMongoAbstractionRepository<TEntity, TRepository, TImplementation>(
        this IServiceCollection services,
        LifeTime repositoryLifeTime)
        where TEntity : class
        where TRepository : class
        where TImplementation : MongoDriverRepository<TEntity>, TRepository
    {
        services.AddAbstractionRepository<TEntity, TRepository, TImplementation>(repositoryLifeTime)
            ;

        return services;
    }
    #endregion
}
