using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2ndSemesterProject.Data;
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
        public IActionResult Get()
        {
            return new BadRequestResult();
        }

        // GET api/{version}/account/5
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            using (var context = new ApplicationDbContext()) {
                var user = context.Users.Single(u => u.Id == id);

                return new JsonResult(user);
            }
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
        public void Delete(Guid id)
        {
            using (var context = new ApplicationDbContext())
            {
                var user = context.Users.Single(u => u.Id == id);

                if (user == null)
                    context.Users.Remove(user);

                context.SaveChanges();
            }
        }
    }
}
