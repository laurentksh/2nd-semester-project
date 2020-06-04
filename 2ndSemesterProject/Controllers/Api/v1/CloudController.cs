using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2ndSemesterProject.Data;
using _2ndSemesterProject.Models;
using _2ndSemesterProject.Models.Database;
using System.Security.Claims;
using _2ndSemesterProject.Controllers;
using _2ndSemesterProject.Models.Api.Cloud;
using System.IO;
using System.Net;
using System.IO.Compression;

namespace _2ndSemesterProject.Controllers.Api.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/cloud")]
    public class ApiCloudController : ControllerBase
    {
        public const long MAX_SIZE_PER_REQUEST = 17179869184; //16GB

        [HttpGet("file/{id}")]
        public IActionResult GetFile(Guid id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
                user = this.GetUser();

            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (!CanAccessFile(file, user))
                return new UnauthorizedResult();

            JsonCloudFile fileJson = new JsonCloudFile
            {
                ElementId = file.FileId.ToString(),
                FileName = file.FileNameWithoutExt,
                FileInfo = $"{file.FileExtension.ToUpper()} file - {file.FileSize} MB", //TODO: Create a file type guesser function.
                DirectUrl = Url.Action(nameof(CloudController), nameof(CloudController.File), file.FileId),
                DownloadUrl = Url.Action(nameof(DownloadFile), nameof(ApiCloudController), file.FileId),
                PreviewUrl = Url.Action(nameof(GetPreviewImage), nameof(ApiCloudController), file.FileId)
            };

            return new JsonResult(file);
        }

        [HttpGet("folder/{id}")]
        public IActionResult GetFolder(Guid id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
                user = this.GetUser();

            CloudFolder folder = dbContext.Folders.Where(f => f.FolderId == id).First();

            if (!CanAccessFolder(folder, user))
                return new UnauthorizedResult();

            JsonCloudFolder folderJson = new JsonCloudFolder
            {
                ElementId = folder.FolderId.ToString(),
                FolderName = folder.FolderName,
                FolderInfo = $"{folder.Files.Count} files. {folder.Childs.Count} folders.",
                DirectUrl = Url.Action(nameof(CloudController), nameof(CloudController.Folder), folder.FolderId),
                DownloadUrl = Url.Action(nameof(DownloadFolder), nameof(ApiCloudController), folder.FolderId)
            };

            return new JsonResult(folder);
        }

        [HttpGet("file/{id}/preview")]
        public IActionResult GetPreviewImage(Guid id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;


            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (!CanAccessFile(file, user))
                return new UnauthorizedResult();

            return new FileStreamResult(FileSystemMiddleman.GetFile(file, "-preview.jpg"), "image/jpeg");
        }

        [HttpGet("file/{id}/download")]
        public IActionResult DownloadFile(Guid id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            if (User.Identity.IsAuthenticated)
                user = this.GetUser();

            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (!CanAccessFile(file, user))
                return new UnauthorizedResult();

            var fs = FileSystemMiddleman.GetFile(file);

            if (fs == null)
                return new NotFoundResult();

            return new FileStreamResult(fs, "application/octet-stream");
        }

        [HttpGet("folder/{id}/download")]
        public IActionResult DownloadFolder(Guid id)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;
            MemoryStream archive = null;

            if (User.Identity.IsAuthenticated)
                user = this.GetUser();

            CloudFolder folder = dbContext.Folders.Where(f => f.FolderId == id).First();

            if (!CanAccessFolder(folder, user))
                return new UnauthorizedResult();

            List<CloudFile> toDownload = new List<CloudFile>();

            //Root files
            foreach (CloudFile file in folder.Files)
                toDownload.Add(file);

            /*foreach (CloudFolder folder in folder.Childs)
                foreach (CloudFile file in folder.Files)
                    toDownload.Add(file);*/

            using (var zipArchive = new ZipArchive(archive, ZipArchiveMode.Create, true)) {
                foreach (CloudFile file in toDownload) {
                    zipArchive.CreateEntryFromFile(FileSystemMiddleman.GetAbsolutePath(file), file.FileName, CompressionLevel.Fastest);
                }
            }

            return new FileStreamResult(archive, "application/zip");
        }

        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFileAsync(Guid parent, List<IFormFile> files)
        {
            //TODO: Create response models

            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            //Check if the user is logged in (i.e can upload a file)
            if (!User.Identity.IsAuthenticated)
                return new UnauthorizedResult();

            user = this.GetUser();

            long maxByteTotalRequestSize = 0;

            foreach (IFormFile file in files) {
                maxByteTotalRequestSize += file.Length;

                if (maxByteTotalRequestSize > MAX_SIZE_PER_REQUEST)
                    return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.FAIL, Body = "REQUEST_MAX_TOTAL_SIZE_REACHED" });

                if (file.Length > user.AccountPlan.FileSizeLimit) { //File too big
                    return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.OK, Body = "FILE_MAX_SIZE_REACHED" });
                }

                string filename = WebUtility.HtmlEncode(file.FileName);

                CloudFile dbFile = new CloudFile()
                {
                    FileName = filename,
                    FileNameWithoutExt = Path.GetFileNameWithoutExtension(filename),
                    FileExtension = Path.GetExtension(filename),
                    FileSize = file.Length,
                    IsPublic = false,
                    Owner = user,
                    Parent = parent == null ? null : dbContext.Folders.Single((x) => x.FolderId == parent),
                };

                await dbContext.Files.AddAsync(dbFile);
                await dbContext.SaveChangesAsync();

                await FileSystemMiddleman.SaveFile(file.OpenReadStream(), user, dbFile);
            }

            await dbContext.SaveChangesAsync();

            return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.OK, Body = "FILE_SAVED" });
        }

        public bool CanAccessFile(CloudFile file, AppUser user, ApplicationDbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = new ApplicationDbContext();

            if (file.IsPublic)
                return true;

            //Boilerplate code for checking if the user is allowed to download the file
            if (!User.Identity.IsAuthenticated && !file.IsPublic)
                return false;
            else {
                //Check if the current user has the permission to download the file

                FolderSharedAccess fsa = dbContext.FolderSharedAccesses
                    .Where(a => a.ReceiverId == user.Id)
                    .Where(a2 => a2.SharedFolderId == file.ParentId)
                    .FirstOrDefault();

                FileSharedAccess fsa2 = dbContext.FileSharedAccesses
                    .Where(b => b.ReceiverId == user.Id)
                    .Where(b2 => b2.SharedFileId == file.FileId)
                    .FirstOrDefault();

                bool fsaValid = false;
                bool fsa2Valid = false;

                if (fsa != null)
                    fsaValid = fsa.ExpirationDate > DateTime.UtcNow || fsa.ExpirationDate == DateTime.MinValue;

                if (fsa2 != null)
                    fsa2Valid = fsa2.ExpirationDate > DateTime.UtcNow || fsa2.ExpirationDate == DateTime.MinValue;

                return fsaValid || fsa2Valid;
            }
        }

        public bool CanAccessFolder(CloudFolder folder, AppUser user, ApplicationDbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = new ApplicationDbContext();

            if (folder.IsPublic)
                return true;

            //Boilerplate code for checking if the user is allowed to access the folder
            if (!User.Identity.IsAuthenticated && !folder.IsPublic)
                return false;
            else {
                //Check if the current user has the permission to access the folder

                FolderSharedAccess fsa = dbContext.FolderSharedAccesses
                    .Where(a => a.ReceiverId == user.Id)
                    .Where(a2 => a2.SharedFolderId == folder.ParentId)
                    .FirstOrDefault();

                bool fsaValid = false;

                if (fsa != null)
                    fsaValid = fsa.ExpirationDate > DateTime.UtcNow || fsa.ExpirationDate == DateTime.MinValue;

                return fsaValid;
            }
        }
    }
}