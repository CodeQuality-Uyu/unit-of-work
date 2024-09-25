using CQ.UnitOfWork.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CQ.UnitOfWork.EfCore.Core;
public abstract class EfCoreContext(DbContextOptions options) :
    DbContext(options),
    IDatabaseContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var contextAssembly = Assembly.GetExecutingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(contextAssembly);
    }

    public virtual void EnsureCreated()
    {
        Database.EnsureCreated();
    }

    public virtual void EnsureDeleted()
    {
        Database.EnsureDeleted();
    }

    public virtual bool Ping(string? collection = null)
    {
        try
        {
            return Database.CanConnect();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error pinging the database: {ex.Message}");
            return false;
        }
    }

    public virtual DbSet<TEntity> GetEntitySet<TEntity>()
        where TEntity : class
    {
        return base.Set<TEntity>();
    }

    public virtual string GetTableName<TEntity>()
    {
        return typeof(TEntity).Name;
    }

    async Task IDatabaseContext.SaveChangesAsync()
    {
        await SaveChangesAsync().ConfigureAwait(false);
    }
}
