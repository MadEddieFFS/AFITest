using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AFI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("0.1", Deprecated = false)]
    public class PolicyHolderController : ControllerBase
    {
    }
}
