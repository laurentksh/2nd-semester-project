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
        public IActionResult GetPreviewImage(Guid id, bool file)
        {
            //TODO: Get file preview from folder (example path: "/<userId>/<fileId>/preview.jpg")

            return new FileStreamResult(null, "Content-Type: image/jpeg");
        }
    }
}