using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Controllers.Api.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountController : ControllerBase
    {
        // GET: api/{version}/account
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/{version}/account/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/{version}/account
        [HttpPost]
        public IActionResult Post()
        {
            return new BadRequestResult();
        }

        // PUT api/{version}/account/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody]string value)
        {

        }

        // DELETE api/{version}/account/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
