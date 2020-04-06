using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2ndSemesterProject.Models.Database
{
    public class AccountPlan
    {
        /// <summary>Account Plan Id</summary>
        [Key]
        public Guid Id { get; set; }
        
        /// <summary>Account Plan name (Free, Premium, etc)</summary>
        public string Name { get; set; }

        /// <summary>If the plan is available (purchasable or not)</summary>
        public AccountPlanState State { get; set; }

        /// <summary>Plan price per month (in CHF)</summary>
        public float PricePerMonth { get; set; }

        /// <summary>Plan price per year (in CHF)</summary>
        public float PricePerYear { get; set; }

        /// <summary>Max storage per account</summary>
        public int GlobalStorageLimit { get; set; }

        /// <summary>Max file size</summary>
        public long FileSizeLimit { get; set; }


        public enum AccountPlanState
        {
            Available = 1,
            Unavailable = 2
        }


        // Inverse properties

        [InverseProperty(nameof(AppUser.AccountPlan))]
        public List<AppUser> Users { get; set; }
    }
}
