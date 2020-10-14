using AFI.API.Controllers.PolicyHolder;
using AFI.API.Controllers.PolicyHolder.Requests;
using AFI.Domain.Repositories.PolicyHolders;
using AFI.Persistance.Contexts;
using AFI.Persistance.Repositories.PolicyHolders;
using AFI.Test.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AFI.Test.API.Controllers.PolicyHolder
{
    public class PolicyHolderControllerTests
    {
        private AFIContext _dbContext;
        private IPolicyHolderRepository _repo;
        private Mock<ILogger<PolicyHolderController>> _logger;
        private PolicyHolderController _controller;


        public PolicyHolderControllerTests()
        {
            this._dbContext = new InMemoryDbContextFactory().GetAFIContext();
            this._repo = new PolicyHolderRepository(this._dbContext);
            this._logger = new Mock<ILogger<PolicyHolderController>>();
            this._controller = new PolicyHolderController(this._repo, this._logger.Object);
        }

        [Fact]
        public async Task PolicyHolderControllerTests_Post_Fail()
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest { };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestResult>();
        }


        [Theory]
        [InlineData("XX - 444444")]
        [InlineData("933444555")]
        [InlineData("X-3334444")]
        [InlineData("XX-22223")]
        [InlineData("XX-22222A")]
        [InlineData("XX-2122222")]
        public async Task PolicyHolderControllerTests_Post_ReferenceValidation_Fail(string ReferenceNumber)
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-40),
                Forename = "Forename",
                Surname = "Surname",
                ReferenceNumber = ReferenceNumber
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestResult>();
        }


        [Theory]
        [InlineData("123@test.com")]
        [InlineData("123@test.co.uk")]
        [InlineData("1234@a.com")]
        [InlineData("1234@a.co.uk")]
        [InlineData("1234@12.co.uk1")]
        [InlineData("1234@12.com1")]
        public async Task PolicyHolderControllerTests_Post_EMailValidation_Fail(string EMail)
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                EMail = EMail,
                Forename = "Forename",
                Surname = "Surname",
                ReferenceNumber = "XX-123456"
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData("1234@test.com")]
        [InlineData("1234@test.co.uk")]
        public async Task PolicyHolderControllerTests_Post_Validation_EMail_Success(string EMail)
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                EMail = EMail,
                Forename = "Forename",
                Surname = "Surname",
                ReferenceNumber = "XX-123456"
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType(typeof(OkResult));
        }

        [Fact]
        public async Task PolicyHolderControllerTests_Post_Validation_DOB_Success()
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-40),
                Forename = "Forename",
                Surname = "Surname",
                ReferenceNumber = "XX-123456"
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType(typeof(OkResult));
        }

        [Theory]
        [InlineData("a")]
        [InlineData("aa")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public async Task PolicyHolderControllerTests_Post_Forename_Fail(string Forename)
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-40),
                Forename = Forename,
                Surname = "Surname",
                ReferenceNumber = "XX-123456"
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestResult>();
        }


        [Theory]
        [InlineData("a")]
        [InlineData("aa")]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
        public async Task PolicyHolderControllerTests_Post_Surname_Fail(string Surname)
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-40),
                Forename = "Test Forename",
                Surname = Surname,
                ReferenceNumber = "XX-123456"
            };

            // Act
            var result = await this._controller.CreatePolicyHolder(request);

            // Assert
            result.Should().NotBeNull().And.BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task PolicyHolderControllerTests_Post_Create_Success()
        {
            // Arrange
            NewPolicyHolderRequest request = new NewPolicyHolderRequest
            {
                DateOfBirth = DateTime.Now.AddYears(-40),
                Forename = "Forename",
                Surname = "Surname",
                ReferenceNumber = "XX-123456"
            };

            // Act

            // Assert
            var result = this._controller.CreatePolicyHolder(request).Result as OkObjectResult;
            result.Should().NotBeNull();
            result.Value.Should().BeOfType(typeof(int))
                .And.Be(expected: 1, because: "It should be the only record in the database and thus first identity created");
        }

    }
}
