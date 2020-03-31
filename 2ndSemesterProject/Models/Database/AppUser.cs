using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2ndSemesterProject.Models.Database
{
    public class AppUser : IdentityUser<Guid>
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        public DateTime BirthDay { get; set; }


        [ForeignKey(nameof(AccountPlan))]
        public Guid AccountPlanId { get; set; }
        
        public AccountPlan AccountPlan { get; set; }


        public DateTime CreationDate { get; set; } = DateTime.Now;


        // Inverse properties

        [InverseProperty(nameof(FolderSharedAccess.Sender))]
        public List<FolderSharedAccess> FolderSharedAccessesSenders { get; set; }

        [InverseProperty(nameof(FolderSharedAccess.Receiver))]
        public List<FolderSharedAccess> FolderSharedAccessesReceiver { get; set; }
    }

    public class AppRole : IdentityRole<Guid>
    {
        
    }
}
