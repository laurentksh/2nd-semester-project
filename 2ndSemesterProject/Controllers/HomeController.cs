using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using _2ndSemesterProject.Models;
using System.Globalization;

namespace _2ndSemesterProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            List<string> CountryList = new List<string>();

            CultureInfo[] cInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var cInfo in cInfoList)
            {
                RegionInfo r = new RegionInfo(cInfo.LCID);

                if (!CountryList.Contains(r.EnglishName))
                    CountryList.Add(r.EnglishName);
            }

            CountryList.Sort();

            ViewBag.CountryList = CountryList.Prepend("Select a country...");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
