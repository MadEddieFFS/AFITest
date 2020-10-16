// <copyright file="PolicyHolderController.cs" company="Mad Eddie Designs">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace AFI.API.Controllers.PolicyHolder
{
    using System.Threading.Tasks;
    using AFI.API.Controllers.PolicyHolder.Requests;
    using AFI.API.Controllers.PolicyHolder.Response;
    using AFI.Domain.Repositories.PolicyHolders;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Swashbuckle.AspNetCore.Annotations;

    /// <summary>
    /// API controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("0.1", Deprecated = false)]
    public class PolicyHolderController : ControllerBase
    {
        private readonly IPolicyHolderRepository policyHolderRepository;
        private readonly ILogger<PolicyHolderController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyHolderController"/> class.
        /// </summary>
        /// <param name="policyHolderRepository">Repository interface injected by .NET Core.</param>
        /// <param name="logger">Global Serilog logger.</param>
        public PolicyHolderController(IPolicyHolderRepository policyHolderRepository, ILogger<PolicyHolderController> logger)
        {
            this.policyHolderRepository = policyHolderRepository;
            this.logger = logger;
        }

        /// <summary>
        /// Receive POST of new policy holder registration.
        /// </summary>
        /// <param name="request">NewPolicyHolderRequest.</param>
        /// <returns>200 or 400</returns>
        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Receive POST of new policy holder registration", Description = "Method used to register/create a new Policy holder", OperationId = "Create", Tags = new[] { "PolicyHolder" })]
        [SwaggerResponse(200, "Validation passed. New account will have been created if succeeded", typeof(NewPolicyHolderResponse))]
        [SwaggerResponse(400, "Validation failed - no further detail will be provided for securty reasons")]
        public async Task<IActionResult> CreatePolicyHolder([FromBody] NewPolicyHolderRequest request)
        {
            // Validation
            if (!request.IsValid)
            {
                // We will give no more detail for security reasons
#pragma warning disable SA1101 // Prefix local calls with this
                return BadRequest();
#pragma warning disable SA1101 // Prefix local calls with this
            }

            // Save
            Domain.Models.PolicyHolders.PolicyHolder result = await this.policyHolderRepository.AddEdit(new Domain.Models.PolicyHolders.PolicyHolder
            {
                DateOfBirth = request.DateOfBirth,
                EMail = request.EMail,
                Forename = request.Forename,
                ReferenceNumber = request.ReferenceNumber,
                Surname = request.Surname,
            });

            this.logger.LogInformation("New policy created");

#pragma warning disable SA1101 // Prefix local calls with this
            return Ok(result.Id);
#pragma warning restore SA1101 // Prefix local calls with this
        }
    }
}
