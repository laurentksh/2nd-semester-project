using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Controllers
{
    [Route("My-Cloud", Name = "Cloud")]
    public class CloudController : Controller
    {
        // GET: My-Cloud/
        [HttpGet("")]
        [HttpGet("Index")]
        public ActionResult Index()
        {
            /*if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");*/

            return View();
        }

        // GET: My-Cloud/File/5
        [HttpGet("File/{id}")]
        public ActionResult File(Guid id)
        {

            return Json(null);
            //return View();
        }

        [HttpGet("Folder/{id}")]
        public ActionResult Folder(Guid id)
        {

            return Json(null);
            //return View();
        }
    }
}