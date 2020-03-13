using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2ndSemesterProject.Models;

namespace _2ndSemesterProject.Models
{
    public class File
    {
        /// <summary>File Id</summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// File name when it was uploaded (DO NOT USE FOR FILE STORING)
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>User uploaded details/notes about the file</summary>
        public string FileDescription { get; set; }
        
        /// <summary>When the file was created/uploaded</summary>
        public DateTime CreationDate { get; set; }
        
        /// <summary>Last time the file was edited</summary>
        public DateTime LastEditDate { get; set; }
        
        /// <summary>In MBytes</summary>
        public long FileSize { get; set; }


        /// <summary>
        /// File's parent (if null, the file is located at root)
        /// </summary>
        public Folder Parent { get; set; }
    }

    public class Folder
    {
        /// <summary>Folder Id</summary>
        public Guid FolderId { get; set; }
        
        /// <summary>Folder name</summary>
        public string FolderName { get; set; }
        
        /// <summary>When the folder was created/uploaded</summary>
        public DateTime CreationDate { get; set; }


        /// <summary>Folder's parent (if null, is a root folder)</summary>
        public Folder Parent { get; set; } = null;
    }

    public class FolderSharedAccess
    {
        public Guid Id { get; set; }
        public DateTime ExpirationDate { get; set; }


        /// <summary>Who shared this folder with you</summary>
        public AppUser Sender { get; set; }

        /// <summary>Who received this folder access</summary>
        public AppUser Receiver { get; set; }

        /// <summary>Shared folder</summary>
        public Folder SharedFolder { get; set; }
    }
}
