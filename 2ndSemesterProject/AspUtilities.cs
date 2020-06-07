using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using _2ndSemesterProject.Data;
using _2ndSemesterProject.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public static async Task<AppUser> GetUser(this ControllerBase controller, UserManager<AppUser> userManager, ApplicationDbContext dbContext)
        {
            var userUncomplete = await userManager.GetUserAsync(controller.User);

            if (userUncomplete == null)
                return null;

            if (!Guid.TryParse(await userManager.GetUserIdAsync(userUncomplete), out Guid userId))
                return null;

            //Include the account plan
            var user = dbContext.Users
                .Include(u => u.AccountPlan)
                .SingleOrDefault(u => u.Id == userId);

            return user;
        }
    }
}
