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
        public long Id { get; set; }
        
        /// <summary>Account Plan name (Free, Premium, etc)</summary>
        public string Name { get; set; }

        /// <summary>If the plan is available (purchasable or not)</summary>
        public AccountPlanState State { get; set; }


        /// <summary>Plan price per month (in CHF)</summary>
        public float PricePerMonth { get; set; }

        /// <summary>Plan price per year (in CHF)</summary>
        public float PricePerYear { get; set; }

        /// <summary>Price reduction applied on the monthly charge (value between 0 and 1)</summary>
        public float ReductionPerMonth { get; set; }

        /// <summary>Price reduction applied on the yearly charge (value between 0 and 1)</summary>
        public float ReductionPerYear { get; set; }


        /// <summary>Max storage per account (In Bytes)</summary>
        public long GlobalStorageLimit { get; set; }

        /// <summary>Max file size (In Bytes)</summary>
        public long FileSizeLimit { get; set; }

        /// <summary>Max file transfer sizes (In Bytes)</summary>
        public long FileTransferSize { get; set; }

        public enum AccountPlanState
        {
            Available = 1,
            Unavailable = 2
        }


        // Inverse properties

        [InverseProperty(nameof(AppUser.AccountPlan))]
        public List<AppUser> Users { get; set; }


        public static AccountPlan GetFreeTier()
        {
            var freeTier = new AccountPlan
            {
                Name = "Free",

                PricePerMonth = 0f,
                PricePerYear = 0f,
                ReductionPerMonth = 0f,
                ReductionPerYear = 0f,

                State = AccountPlanState.Available,
                FileSizeLimit = 500_000_000L,
                GlobalStorageLimit = 15_000_000_000L,
                FileTransferSize = 2_684_354_560L //500MB * 5
            };

            return freeTier;
        }

        public static AccountPlan GetPlusTier()
        {
            var plusTier = new AccountPlan
            {
                Name = "Plus",

                PricePerMonth = 4.99f,
                PricePerYear = 59.99f,
                ReductionPerMonth = 0f,
                ReductionPerYear = 0f,//0.25f,

                State = AccountPlanState.Available,
                FileSizeLimit = 5_000_000_000L,
                GlobalStorageLimit = 250_000_000_000L,
                FileTransferSize = 26_843_545_600L //5GB * 5
            };

            return plusTier;
        }

        public static AccountPlan GetProTier()
        {
            var proTier = new AccountPlan
            {
                Name = "Pro",

                PricePerMonth = 19.99f,
                PricePerYear = 239.99f,
                ReductionPerMonth = 0f,
                ReductionPerYear = 0f,//0.15f,

                State = AccountPlanState.Available,
                FileSizeLimit = 20_000_000_000L,
                GlobalStorageLimit = 1000_000_000_000L,
                FileTransferSize = 107_374_182_400L //20GB * 5
            };

            return proTier;
        }
    }
}
