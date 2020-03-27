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

        // GET: My-Cloud/Details/5
        [HttpGet("Details/{id}")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: My-Cloud/Create
        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: My-Cloud/Edit/5
        [HttpGet("Edit/{id}")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // GET: My-Cloud/Delete/5
        [HttpGet("Delete/{id}")]
        public ActionResult Delete(int id)
        {
            return View();
        }


        // POST: My-Cloud/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: My-Cloud/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: My-Cloud/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}