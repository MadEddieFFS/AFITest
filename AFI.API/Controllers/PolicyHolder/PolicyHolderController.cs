using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AFI.API.Controllers.PolicyHolder.Requests;
using AFI.API.Controllers.PolicyHolder.Response;
using AFI.Domain.Repositories.PolicyHolders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Settings.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace AFI.API.Controllers.PolicyHolder
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("0.1", Deprecated = false)]
    public class PolicyHolderController : ControllerBase
    {
        IPolicyHolderRepository _policyHolderRepository;
        ILogger<PolicyHolderController> _logger;

        public PolicyHolderController(IPolicyHolderRepository PolicyHolderRepository, ILogger<PolicyHolderController> logger)
        {
            this._policyHolderRepository = PolicyHolderRepository;
            this._logger = logger;
        }

        /// <summary>
        /// Receive POST of new policy holder registration
        /// </summary>
        /// <param name="request">NewPolicyHolderRequest</param>
        /// <returns></returns>
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
                return BadRequest();
            }

            // Save
            Domain.Models.PolicyHolders.PolicyHolder Result = await this._policyHolderRepository.AddEdit(new Domain.Models.PolicyHolders.PolicyHolder
            {
                DateOfBirth = request.DateOfBirth,
                EMail = request.EMail,
                Forename = request.Forename,
                ReferenceNumber = request.ReferenceNumber,
                Surname = request.Surname
            });

            return Ok(Result.Id);
        }
    }
}
