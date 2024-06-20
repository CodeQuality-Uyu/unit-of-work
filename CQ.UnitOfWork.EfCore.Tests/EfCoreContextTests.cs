﻿using CQ.UnitOfWork.EfCore.Core;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace CQ.UnitOfWork.EfCore.Tests
{
    [TestClass]
    public class EfCoreContextTests
    {
        private readonly DbConnection _connection;
        private readonly EfCoreContext _testContext;

        public EfCoreContextTests()
        {
            this._connection = new SqliteConnection("Filename=:memory:");
            var contextOptions = new DbContextOptionsBuilder<TestContext>().UseSqlite(this._connection).Options;
            this._testContext = new TestContext(contextOptions);
        }

        [TestInitialize]
        public void SetUp()
        {
            this._connection.Open();
            this._testContext.Database.EnsureCreated();
        }

        [TestCleanup]
        public void CleanUp()
        {
            this._testContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void GetTableName_WhenSetNameDifferentThanType_ShouldReturnPropertyName()
        {
            var name = this._testContext.GetTableName<TestUser>();

            name.Should().NotBeNull();
            name.Should().Be("Users");
        }
    }
}
