using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _2ndSemesterProject.Data;
using _2ndSemesterProject.Models.Database;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject
{
    /// <summary>
    /// Mostly extensions methods used to facilitate the use of ASP.NET Core.
    /// </summary>
    public static class AspUtilities
    {
        /// <summary>
        /// Returns the user currently logged in.
        /// </summary>
        /// <returns>AppUser account</returns>
        public static AppUser GetUser(this ControllerBase controller)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();

            return dbContext.Users
                .Single(u => u.Id == Guid.Parse(controller.User.FindFirst(ClaimTypes.NameIdentifier).Value));
        }
    }
}
