using AFI.API.Controllers.PolicyHolder;
using AFI.API.Controllers.PolicyHolder.Requests;
using AFI.Domain.Repositories.PolicyHolders;
using AFI.Persistance.Contexts;
using AFI.Persistance.Repositories.PolicyHolders;
using AFI.Test.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AFI.Test.API.Controllers.PolicyHolder
{
    public class PolicyHolderControllerTests
    {
        private AFIContext _dbContext;
        private IPolicyHolderRepository _repo;
        private Mock<ILogger<PolicyHolderController>> _logger;
        private PolicyHolderController _controller;


        [SetUp]
        public void Setup()
        {
            this._dbContext = new InMemoryDbContextFactory().GetAFIContext();
            this._repo = new PolicyHolderRepository(this._dbContext);
            this._logger = new Mock<ILogger<PolicyHolderController>>();
            this._controller = new PolicyHolderController(this._repo, this._logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // NOOP
        }

        [Test]
        public async Task PolicyHolderControllerTests_Post_Fail()
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest { };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(result);
        }


    }
}
