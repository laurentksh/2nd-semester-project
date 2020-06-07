using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2ndSemesterProject.Models.Api.Cloud
{
    public class JsonCloudFile
    {
        public string ElementId; //GUID
        public string FileName;
        public string FileNameWithoutExt;
        public string FileInfo; //E.g: PNG File - 5KB
        public bool IsPublic;
        public DateTime CreationDate;
        public DateTime LastEditDate;
        public long FileSize;
        public string ParentId;
        public string OwnerId;

        public string DirectUrl;
        public string DownloadUrl;
        public string PreviewUrl;
    }

    public class JsonCloudFolder
    {
        public string ElementId; //GUID
        public string FolderName;
        public string FolderInfo; //E.g: Folder - 36MB
        public bool IsPublic;
        public DateTime CreationDate;
        public string ParentId;
        public string OwnerId;

        public string DirectUrl;
        public string DownloadUrl;
    }

    public class JsonFolderSharedAccess
    {
        public string Id;

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        public DateTime ExpirationDate;


        /// <summary>Who shared this folder with you</summary>
        public string Sender;

        /// <summary>Who received this folder access</summary>
        public string Receiver;

        /// <summary>Shared folder</summary>
        public string SharedFolder;
    }

    public class JsonFileSharedAccess
    {
        public Guid Id;

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        public DateTime ExpirationDate;

        /// <summary>Who shared this folder with you</summary>
        public string SenderId;

        /// <summary>Who received this folder access</summary>
        public string ReceiverId;

        /// <summary>Shared folder</summary>
        public string SharedFileId;
    }
}
