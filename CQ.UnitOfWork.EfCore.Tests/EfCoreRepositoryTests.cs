using CQ.UnitOfWork.EfCore.Core;
using CQ.Utility;
using Microsoft.EntityFrameworkCore;

namespace CQ.UnitOfWork.EfCore.Tests
{
    [TestClass]
    [TestCategory("")]
    public sealed class EfCoreRepositoryTests
    {
        private TestDbContext _context = DbContextFactory.BuildTestContext();
        private EfCoreRepository<TestUser> _efCoreRepository;

        public EfCoreRepositoryTests()
        {
            _efCoreRepository = new(_context);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task CreateAsync_WhenNewUser_ShouldSaveNewUser()
        {
            var newUser = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var result = await _efCoreRepository.CreateAsync(newUser).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.Id.Should().Be(newUser.Id);
            result.Name.Should().Be("some name");
        }

        [TestMethod]
        public async Task GetPagedAsync_WhenPageAndPageSize_ShouldReturnPagination()
        {
            var newUser = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser2 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };
            using var setContext = DbContextFactory.BuildTestContext();
            await setContext.AddRangeAsync(newUser, newUser2).ConfigureAwait(false);
            await setContext.SaveChangesAsync().ConfigureAwait(false);

            var result = await _efCoreRepository.GetPagedAsync(page: 1, pageSize: 1).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.TotalItems.Should().Be(1);
            result.TotalCount.Should().Be(2);
            result.TotalPages.Should().Be(2);
            result.HasNext.Should().BeTrue();
            result.HasPrevious.Should().BeFalse();
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(1);
            result.Items.Should().Contain(newUser);
        }

        [TestMethod]
        public async Task GetPagedAsync_WhenWithDecimalPageCount_ShouldReturnTotalPagesBiggest()
        {
            var newUser = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser2 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            var newUser3 = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };

            using var setContext = DbContextFactory.BuildTestContext();
            await setContext.AddRangeAsync(newUser, newUser2, newUser3).ConfigureAwait(false);
            await setContext.SaveChangesAsync().ConfigureAwait(false);

            var result = await _efCoreRepository.GetPagedAsync(page: 1, pageSize: 2).ConfigureAwait(false);

            result.Should().NotBeNull();
            result.TotalItems.Should().Be(2);
            result.TotalCount.Should().Be(3);
            result.TotalPages.Should().Be(2);
            result.HasNext.Should().BeTrue();
            result.HasPrevious.Should().BeFalse();
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.Items.Should().Contain(newUser);
            result.Items.Should().Contain(newUser2);
        }

        [TestMethod]
        public async Task UpdateByIdAsync_WhenDataExists_ShouldUpdateOnDataBase()
        {
            var user = new TestUser
            {
                Id = Db.NewId(),
                Name = "some name"
            };
            using var setContext = DbContextFactory.BuildTestContext();
            await setContext.AddAsync(user).ConfigureAwait(false);
            await setContext.SaveChangesAsync().ConfigureAwait(false);

            await _efCoreRepository.UpdateAndSaveByIdAsync(user.Id, new { Name = "updated" }).ConfigureAwait(false);

            using var otherContext = DbContextFactory.BuildTestContext();
            var userUpdated = await otherContext.Users.FirstAsync(u => u.Id == user.Id).ConfigureAwait(false);

            userUpdated.Should().NotBeNull();
            userUpdated.Name.Should().Be("updated");
        }
    }

    internal sealed record class TestUser
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

    internal sealed class TestDbContext : EfCoreContext
    {
        public DbSet<TestUser> Users { get; set; }

        public TestDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}