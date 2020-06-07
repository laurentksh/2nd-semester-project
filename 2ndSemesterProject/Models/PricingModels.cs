using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2ndSemesterProject.Models.Database;

namespace _2ndSemesterProject.Models
{
    public class PricingModels
    {
        public AccountPlan FreeTier { get; set; }

        public AccountPlan PlusTier { get; set; }

        public AccountPlan ProTier { get; set; }
    }
}
