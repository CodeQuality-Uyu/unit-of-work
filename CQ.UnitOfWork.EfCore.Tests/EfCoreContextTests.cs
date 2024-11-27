namespace CQ.UnitOfWork.EfCore.Tests;

[TestClass]
public class EfCoreContextTests
{
    private static readonly TestDbContext _context = DbContextFactory.BuildTestContext();

    [TestMethod]
    public void GetTableName_WhenSetNameDifferentThanType_ShouldReturnPropertyName()
    {
        var name = _context.GetTableName<TestUser>();

        name.Should().NotBeNull();
        name.Should().Be("Users");
    }
}
