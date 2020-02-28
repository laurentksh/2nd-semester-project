using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Controllers
{
    [Route("/my-cloud", Name = "cloud")]
    public class CloudController : Controller
    {
        // GET: my-cloud
        [Route("/")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: my-cloud/Details/5
        [Route("/Details/{id}")]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: my-cloud/Create
        [Route("/Create")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: my-cloud/Edit/5
        [Route("/Edit/{id}")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // GET: my-cloud/Delete/5
        [Route("/Delete/{id}")]
        public ActionResult Delete(int id)
        {
            return View();
        }


        // POST: my-cloud/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Create")]
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

        // POST: my-cloud/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Edit/{id}")]
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

        // POST: my-cloud/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/Delete/{id}")]
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