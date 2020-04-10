using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2ndSemesterProject.Models
{
    public class JsonCloudFile
    {
        public string ElementId; //GUID
        public string FileName;
        public string FileInfo; //E.g: PNG File - 5KB
        public string DirectUrl;
        public string DownloadUrl;
        public string PreviewUrl;
    }

    public class JsonCloudFolder
    {
        public string ElementId; //GUID
        public string FolderName;
        public string FolderInfo; //E.g: Folder - 36MB
        public string DirectUrl;
        public string DownloadUrl;
    }
}
