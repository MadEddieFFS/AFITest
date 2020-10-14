using AFI.Domain.Models.PolicyHolders;
using AFI.Domain.Repositories.PolicyHolders;
using AFI.Persistance.Contexts;
using AFI.Persistance.Repositories.PolicyHolders;
using AFI.Test.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AFI.Test.Persistence
{
    public class BaseRepositoryTest
    {
        private IServiceProvider _provider;

        public BaseRepositoryTest()
        {
            var services = new ServiceCollection();
            services.AddDbContext<AFIContext>(options => options.UseInMemoryDatabase($"afi-db-{Guid.NewGuid()}"), ServiceLifetime.Transient);
            _provider = services.BuildServiceProvider();
        }


        [Fact]
        public async Task BaseRepositoryTest_FindByID()
        {
            // Arrange
            var db = _provider.GetService<AFIContext>();
            var repo = new PolicyHolderRepository(db);

            int TestID = 1;
            Guid CreatedByUserId = Guid.Parse("D9FE4F17-306D-4B40-A2F1-0A960ECC4A8C");
            string ExpectedReference = "TEST 1";

            db.PolicyHolder.Add(new Domain.Models.PolicyHolders.PolicyHolder
            {
                CreatedByUserId = CreatedByUserId,
                CreatedUTC = DateTime.UtcNow.AddDays(-1),
                Id = TestID,
                ReferenceNumber = ExpectedReference
            });
            db.SaveChanges();

            // Act
            var results = await repo.FindByIdAsync(TestID);

            // Assert
            results.Should().NotBeNull();
            results.Id.Should().Equals(TestID);
            results.ReferenceNumber.Should().Equals(ExpectedReference);
        }


        [Fact]
        public async Task BaseRepository_Insert()
        {
            // Arrange
            var db = _provider.GetService<AFIContext>();
            var repo = new PolicyHolderRepository(db);

            string ExpectedReference = "TEST 1";
            PolicyHolder PolicyHolder = new PolicyHolder
            {
                Forename = "Test Forename",
                Surname = "Test Surname",
                ReferenceNumber = ExpectedReference
            };

            // Act
            Action act = () => repo.Insert(PolicyHolder).Wait();

            // Assert
            act.Should().NotThrow<Exception>();
        }
    }
}
