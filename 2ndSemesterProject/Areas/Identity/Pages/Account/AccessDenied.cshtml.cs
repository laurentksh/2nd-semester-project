﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace _2ndSemesterProject.Areas.Identity.Pages.Account
{
    public class AccessDeniedModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}

