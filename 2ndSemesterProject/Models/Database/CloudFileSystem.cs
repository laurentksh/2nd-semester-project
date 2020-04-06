using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _2ndSemesterProject.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2ndSemesterProject.Models.Database
{
    public class File
    {
        /// <summary>File Id</summary>
        [Key]
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


        [ForeignKey(nameof(ParentId))]
        public Guid ParentId { get; set; }

        /// <summary>
        /// File's parent (if null, the file is located at root)
        /// </summary>
        
        public Folder Parent { get; set; }
    }

    public class Folder
    {
        /// <summary>Folder Id</summary>
        [Key]
        public Guid FolderId { get; set; }
        
        /// <summary>Folder name</summary>
        public string FolderName { get; set; }
        
        /// <summary>When the folder was created/uploaded</summary>
        public DateTime CreationDate { get; set; }


        [ForeignKey(nameof(Parent))]
        public Guid ParentId { get; set; }

        /// <summary>Folder's parent (if null, is a root folder)</summary>
        public Folder Parent { get; set; } = null;

        
        // Inverse properties

        [InverseProperty(nameof(Parent))]
        public List<Folder> Childs { get; set; }

        [InverseProperty(nameof(File.Parent))]
        public List<File> Files { get; set; }
    }

    public class FolderSharedAccess
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public DateTime ExpirationDate { get; set; }


        [ForeignKey(nameof(Sender)), Column(Order = 0)]
        public Guid SenderId { get; set; }

        [ForeignKey(nameof(Receiver)), Column(Order = 1)]
        public Guid ReceiverId { get; set; }

        [ForeignKey(nameof(SharedFolder)), Column(Order = 2)]
        public Guid SharedFolderId { get; set; }


        /// <summary>Who shared this folder with you</summary>
        public AppUser Sender { get; set; }

        /// <summary>Who received this folder access</summary>
        public AppUser Receiver { get; set; }

        /// <summary>Shared folder</summary>
        public Folder SharedFolder { get; set; }
    }
}
