using CQ.UnitOfWork.Api.EFCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CQ.EfCore.Migrations
{
    public class ConcreteContextFactory : IDesignTimeDbContextFactory<ConcreteContext>
    {
        public ConcreteContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ConcreteContext>()
                .UseSqlServer("Server=localhost;Database=UnitOfWork; Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine)
                .Options;

            return new ConcreteContext(options);
        }
    }
}
