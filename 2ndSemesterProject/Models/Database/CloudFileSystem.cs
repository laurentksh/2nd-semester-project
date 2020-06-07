using System;
using System.Collections.Generic;
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
        [Required(AllowEmptyStrings = false)]
        public string FileName { get; set; }

        /// <summary>
        /// File name when it was uploaded, without it's file extension.
        /// </summary>
        public string FileNameWithoutExt { get; set; }
        
        /// <summary>File extension (can be null)</summary>
        public string FileExtension { get; set; }

        /// <summary>User uploaded details/notes about the file</summary>
        public string FileDescription { get; set; }

        /// <summary>
        /// Specifies whether if the file can be viewed and downloaded by anyone on the internet or not.
        /// </summary>
        [Required]
        public bool IsPublic { get; set; } = false;

        /// <summary>UTC. When the file was created/uploaded</summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        /// <summary>UTC. Last time the file was edited</summary>
        [Required]
        public DateTime LastEditDate { get; set; } = DateTime.UtcNow;
        
        /// <summary>In Bytes</summary>
        [Required]
        public long FileSize { get; set; }


        public Guid? ParentId { get; set; }
        
        public Guid OwnerId { get; set; }

        /// <summary>File's parent (if null, the file is located at root)</summary>
        [ForeignKey(nameof(ParentId))]
        public CloudFolder Parent { get; set; }

        /// <summary></summary>
        [ForeignKey(nameof(OwnerId))]
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

        /// <summary>UTC. When the folder was created/uploaded</summary>
        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;

        
        public Guid ParentId { get; set; }

        public Guid OwnerId { get; set; }

        /// <summary>Folder's parent (if null, is a root folder)</summary>
        [ForeignKey(nameof(ParentId))]
        public CloudFolder Parent { get; set; } = null;

        /// <summary>Folder's owner</summary>
        [ForeignKey(nameof(OwnerId))]
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

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        [Required]
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;

        public bool HasWritePermission { get; set; } = false;

        
        public Guid SenderId { get; set; }
        
        public Guid ReceiverId { get; set; }
        
        public Guid SharedFolderId { get; set; }

        /// <summary>Who shared this folder with you</summary>
        [ForeignKey(nameof(SenderId))/*, Column(Order = 0)*/]
        public AppUser Sender { get; set; }

        /// <summary>Who received this folder access</summary>
        [ForeignKey(nameof(ReceiverId))/*, Column(Order = 1)*/]
        public AppUser Receiver { get; set; }

        /// <summary>Shared folder</summary>
        [ForeignKey(nameof(SharedFolderId))/*, Column(Order = 2)*/]
        public CloudFolder SharedFolder { get; set; }
    }

    public class FileSharedAccess
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>UTC. DateTime.MinValue: No expiration</summary>
        [Required]
        public DateTime ExpirationDate { get; set; } = DateTime.MinValue;

        
        public Guid SenderId { get; set; }

        public Guid ReceiverId { get; set; }
        
        public Guid SharedFileId { get; set; }

        /// <summary>Who shared this file with you</summary>
        [ForeignKey(nameof(SenderId))/*, Column(Order = 0)*/]
        public AppUser Sender { get; set; }

        /// <summary>Who received this file access</summary>
        [ForeignKey(nameof(ReceiverId))/*, Column(Order = 1)*/]
        public AppUser Receiver { get; set; }

        /// <summary>Shared file</summary>
        [ForeignKey(nameof(SharedFileId))/*, Column(Order = 2)*/]
        public CloudFile SharedFile { get; set; }
    }
}
