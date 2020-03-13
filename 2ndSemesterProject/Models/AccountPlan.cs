using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2ndSemesterProject.Models
{
    public class AccountPlan
    {
        /// <summary>Account Plan Id</summary>
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
    }
}
