using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2ndSemesterProject.Models.Api.Cloud
{
    public class ResponseModel
    {
        public Status_ Status { get; set; }
        public string Body { get; set; }

        public enum Status_
        {
            OK,
            FAIL
        }
    }
}
