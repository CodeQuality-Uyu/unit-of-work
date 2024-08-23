using CQ.Extensions.ServiceCollection;
using CQ.UnitOfWork.Abstractions;
using CQ.UnitOfWork.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CQ.UnitOfWork.Configuration;
public static class UnitOfWorkConfig
{
    public static IServiceCollection AddUnitOfWork(
        this IServiceCollection services,
        LifeTime unitOfWorkServiceLifeTime)
    {
        services.AddService<IUnitOfWork, UnitOfWorkService>(unitOfWorkServiceLifeTime);

        return services;
    }
}
