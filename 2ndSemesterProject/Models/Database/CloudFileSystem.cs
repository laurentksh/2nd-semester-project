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
    public class CloudFile
    {
        /// <summary>File Id</summary>
        [Key]
        public Guid FileId { get; set; }

        /// <summary>
        /// File name when it was uploaded (DO NOT USE FOR FILE STORING)
        /// </summary>
        [Required]
        public string FileName { get; set; }

        /// <summary>
        /// File name when it was uploaded, without it's file extension.
        /// </summary>
        public string FileNameWithoutExt { get; set; }
        
        /// <summary>
        /// File extension (can be null)
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>User uploaded details/notes about the file</summary>
        public string FileDescription { get; set; }

        /// <summary>Specifies whether if the file can be viewed and downloaded by anyone on the internet or not.</summary>
        [Required]
        public bool IsPublic { get; set; } = false;

        /// <summary>When the file was created/uploaded</summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>Last time the file was edited</summary>
        [Required]
        public DateTime LastEditDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>In MBytes</summary>
        [Required]
        public long FileSize { get; set; }


        [ForeignKey(nameof(Parent))]
        public Guid ParentId { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        /// <summary>File's parent (if null, the file is located at root)</summary>
        public CloudFolder Parent { get; set; }
        
        /// <summary></summary>
        public AppUser Owner { get; set; }
    }

    public class CloudFolder
    {
        /// <summary>Folder Id</summary>
        [Key]
        public Guid FolderId { get; set; }
        
        /// <summary>Folder name</summary>
        [Required]
        public string FolderName { get; set; }

        /// <summary>Specifies whether if the folder can be viewed and downloaded by anyone on the internet or not.</summary>
        [Required]
        public bool IsPublic { get; set; } = false;

        /// <summary>When the folder was created/uploaded</summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;


        [ForeignKey(nameof(Parent))]
        public Guid ParentId { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        /// <summary>Folder's parent (if null, is a root folder)</summary>
        public CloudFolder Parent { get; set; } = null;

        /// <summary>Folder's owner</summary>
        public AppUser Owner { get; set; }


        // Inverse properties

        [InverseProperty(nameof(Parent))]
        public List<CloudFolder> Childs { get; set; }

        [InverseProperty(nameof(CloudFile.Parent))]
        public List<CloudFile> Files { get; set; }
    }

    public class FolderSharedAccess
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>DateTime.MinValue: No expiration</summary>
        [Required]
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;


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
        public CloudFolder SharedFolder { get; set; }
    }

    public class FileSharedAccess
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>DateTime.MinValue: No expiration</summary>
        [Required]
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;
        

        [ForeignKey(nameof(Sender)), Column(Order = 0)]
        public Guid SenderId { get; set; }

        [ForeignKey(nameof(Receiver)), Column(Order = 1)]
        public Guid ReceiverId { get; set; }

        [ForeignKey(nameof(SharedFile)), Column(Order = 2)]
        public Guid SharedFileId { get; set; }


        /// <summary>Who shared this folder with you</summary>
        public AppUser Sender { get; set; }

        /// <summary>Who received this folder access</summary>
        public AppUser Receiver { get; set; }

        /// <summary>Shared folder</summary>
        public CloudFile SharedFile { get; set; }
    }
}
