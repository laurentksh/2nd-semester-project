using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Controllers
{
    [Route("/Security", Name = "Security")]
    public class SecurityController : Controller
    {
        [Route("Login")]
        public IActionResult Login()
        {
            return View();

        }
        [Route("Register")]
        public IActionResult Register() 
        {
            return View();

        }
        [Route("ResetPasswords")]
        public IActionResult ResetPasswords()
        {
            return View();

        }
    }  
}