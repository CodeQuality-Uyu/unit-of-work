using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CQ.UnitOfWork.EfCore.Tests;

internal static class DbContextFactory
{
    private static SqliteConnection _connection = new SqliteConnection("Filename=:memory:");
    
    public static TestDbContext BuildTestContext()
    {
        return BuildDbContext<TestDbContext>();
    }

    public static TDbContext BuildDbContext<TDbContext>()
        where TDbContext : DbContext
    {
        var contextOptions = new DbContextOptionsBuilder<TDbContext>().UseSqlite(_connection).Options;
        var context = (TDbContext)Activator.CreateInstance(typeof(TDbContext), contextOptions);

        _connection.Open();
        context.Database.EnsureCreated();

        return context;
    }
}
