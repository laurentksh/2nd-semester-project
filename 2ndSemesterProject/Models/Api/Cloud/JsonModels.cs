using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2ndSemesterProject.Models.Api.Cloud
{
    public class JsonCloudFile
    {
        public string ElementId { get; set; } //GUID
        public string FileName { get; set; }
        public string FileNameWithoutExt { get; set; }
        public string FileInfo { get; set; } //E.g: PNG File - 5KB
        public bool IsPublic { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastEditDate { get; set; }
        public long FileSize { get; set; }
        public string ParentId { get; set; }
        public string OwnerId { get; set; }

        public string DirectUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string PreviewUrl { get; set; }
    }

    public class JsonCloudFolder
    {
        public string ElementId { get; set; } //GUID
        public string FolderName { get; set; }
        public string FolderInfo { get; set; } //E.g: Folder - 36MB
        public bool IsPublic { get; set; }
        public DateTime CreationDate { get; set; }
        public string ParentId { get; set; }
        public string OwnerId { get; set; }

        public string DirectUrl { get; set; }
        public string DownloadUrl { get; set; }
    }

    public class JsonFolderSharedAccess
    {
        public string Id { get; set; }

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        public DateTime ExpirationDate { get; set; }


        /// <summary>Who shared this folder with you</summary>
        public string Sender { get; set; }

        /// <summary>Who received this folder access</summary>
        public string Receiver { get; set; }

        /// <summary>Shared folder</summary>
        public string SharedFolder { get; set; }
    }

    public class JsonFileSharedAccess
    {
        public string Id { get; set; }

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>Who shared this folder with you</summary>
        public string SenderId { get; set; }

        /// <summary>Who received this folder access</summary>
        public string ReceiverId { get; set; }

        /// <summary>Shared folder</summary>
        public string SharedFileId { get; set; }
    }
}
