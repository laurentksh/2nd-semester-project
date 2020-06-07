using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _2ndSemesterProject.Models.Api.Cloud
{
    public class UploadFileModel
    {
        [FromForm(Name = "ParentFolder")]
        public string FolderId { get; set; }

        [FromForm(Name = "File")]
        public IFormFileCollection Files { get; set; }
    }
}
