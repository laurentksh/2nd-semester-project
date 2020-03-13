using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace _2ndSemesterProject.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDay { get; set; }

        public AccountPlan AccountPlan { get; set; }

        public DateTime CreationDate { get; set; }
    }

    public class AppRole : IdentityRole<Guid>
    {
        
    }
}
