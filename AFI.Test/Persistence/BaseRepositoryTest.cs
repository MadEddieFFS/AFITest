using AFI.Domain.Models.PolicyHolders;
using AFI.Domain.Repositories.PolicyHolders;
using AFI.Persistance.Contexts;
using AFI.Persistance.Repositories.PolicyHolders;
using AFI.Test.TestHelpers;
using Castle.Core.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AFI.Test.Persistence
{
    [TestFixture]
    public class BaseRepositoryTest
    {
        private AFIContext _dbContext;
        private IPolicyHolderRepository _repo;

        [SetUp]
        public void Setup()
        {
            _dbContext = new InMemoryDbContextFactory().GetAFIContext();
            _repo = new PolicyHolderRepository(this._dbContext);
        }


        [Test]
        public async Task BaseRepositoryTest_FindByID()
        {
            // Arrange
            Guid TestID = Guid.Parse("0C05CA6D-AC82-4577-BCF3-9EBEC29F5268");
            Guid CreatedByUserId = Guid.Parse("D9FE4F17-306D-4B40-A2F1-0A960ECC4A8C");
            string ExpectedReference = "TEST 1";

            _dbContext.PolicyHolder.Add(new Domain.Models.PolicyHolders.PolicyHolder
            {
                CreatedByUserId = CreatedByUserId,
                CreatedUTC = DateTime.UtcNow.AddDays(-1),
                Id = TestID,
                ReferenceNumber = ExpectedReference
            });
            _dbContext.SaveChanges();

            // Act
            var results = await _repo.FindByIdAsync(TestID);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Id, TestID);
            Assert.AreEqual(results.ReferenceNumber, ExpectedReference);
        }


        [Test]
        public void BaseRepository_Insert()
        {
            // Arrange
            string ExpectedReference = "TEST 1";
            PolicyHolder PolicyHolder = new PolicyHolder
            {
                Forename = "Test Forename",
                Surname = "Test Surname",
                ReferenceNumber = ExpectedReference
            };

            // Act
            
            // Assert
            Assert.DoesNotThrowAsync(() => { return _repo.Insert(PolicyHolder); });
        }
    }
}
