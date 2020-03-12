using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using _2ndSemesterProject.Models;
using System.Globalization;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace _2ndSemesterProject.Controllers
{
    [Route("/", Name = "Home", Order = -1)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IActionDescriptorCollectionProvider _provider;

        public HomeController(ILogger<HomeController> logger, IActionDescriptorCollectionProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("Debug/AllViews")]
        public IActionResult AllViews()
        {
            var routes = _provider.ActionDescriptors.Items.ToList();

            ViewBag.AllViews = routes;

            return View();
        }

        [Route("Contact")]
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

        [Route("Error", Name = "Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
