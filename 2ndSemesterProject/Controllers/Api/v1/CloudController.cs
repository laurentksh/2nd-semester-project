using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Controllers.Api.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CloudController : ControllerBase
    {
        [HttpGet("preview/{id}")]
        public string GetPreviewImage(Guid id, bool file)
        {
            return "/null";
        }
    }
}