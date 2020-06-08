using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using _2ndSemesterProject.Data;
using _2ndSemesterProject.Models.Database;
using _2ndSemesterProject.Models.Api.Cloud;
using System.IO;
using System.Net;
using System.IO.Compression;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace _2ndSemesterProject.Controllers.Api.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("/api/v{version:apiVersion}/cloud")]
    public class ApiCloudController : ControllerBase
    {
        private ApplicationDbContext dbContext = null;
        private UserManager<AppUser> userManager = null;

        public ApiCloudController(ApplicationDbContext dbContext_, UserManager<AppUser> userManager_)
        {
            dbContext = dbContext_;
            userManager = userManager_;
        }

        [HttpGet("file/{id}")]
        public async Task<IActionResult> GetFile(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            CloudFile file = dbContext.Files
                .Include(f => f.Owner)
                .Include(f => f.Parent)
                .SingleOrDefault(f => f.FileId == Guid.Parse(id));

            if (!CanAccessFile(file, user, dbContext))
                return new ForbidResult();

            JsonCloudFile fileJson = GetFileAsJson(file);

            return new JsonResult(file);
        }

        [HttpGet("folder/{id}")]
        public async Task<IActionResult> GetFolder(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            if (id == "root") {
                if (user == null) //Check if the user is connected.
                    return new ForbidResult();

                JsonCloudFolder jsonCloudFolder = GetRootFolderAsJson(user);

                return new JsonResult(jsonCloudFolder);
            } else {
                CloudFolder folder = GetFolderFromId(id, dbContext);

                if (!CanAccessFolder(folder, user, dbContext))
                    return new ForbidResult();

                JsonCloudFolder folderJson = GetFolderAsJson(folder);

                return new JsonResult(folderJson);
            }
        }

        [HttpGet("file/{id}/childs/")]
        public async Task<IActionResult> GetChildFiles(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            CloudFolder parent = GetFolderFromId(id, dbContext);

            if (id != "root" && !CanAccessFolder(parent, user, dbContext))
                return new ForbidResult();

            List<CloudFile> files = GetFilesFromParent(parent, dbContext);
            List<JsonCloudFile> filesJson = new List<JsonCloudFile>();

            foreach (var file in files) {
                filesJson.Add(GetFileAsJson(file));
            }

            return new JsonResult(filesJson);
        }

        [HttpGet("folder/{id}/childs/")]
        public async Task<IActionResult> GetChildFolders(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            CloudFolder parent = GetFolderFromId(id, dbContext);

            if (id != "root" && !CanAccessFolder(parent, user, dbContext))
                return new ForbidResult();

            List<CloudFolder> folders = GetFoldersFromId(id, dbContext);
            List<JsonCloudFolder> foldersJson = new List<JsonCloudFolder>();

            foreach (var folder in folders) {
                foldersJson.Add(GetFolderAsJson(folder));
            }

            return new JsonResult(foldersJson);
        }

        [HttpGet("file/{id}/preview", Name = "GetPreviewImage")]
        public async Task<IActionResult> GetPreviewImage(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            CloudFile file = dbContext.Files
                .Include(f => f.Owner)
                .Include(f => f.Parent)
                .SingleOrDefault(f => f.FileId == Guid.Parse(id));

            if (file == null)
                return new NotFoundResult();

            if (!CanAccessFile(file, user, dbContext))
                return new ForbidResult();

            return new FileStreamResult(FileSystemMiddleman.GetFile(file, "-preview.jpg"), "image/jpeg");
        }

        [HttpGet("file/{id}/download")]
        public async Task<IActionResult> DownloadFile(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            CloudFile file = dbContext.Files
                .Include(f => f.Owner)
                .Include(f => f.Parent)
                .SingleOrDefault(f => f.FileId == Guid.Parse(id));

            if (file == null)
                return new NotFoundResult();

            if (!CanAccessFile(file, user, dbContext))
                return new ForbidResult();

            var fs = FileSystemMiddleman.GetFile(file);

            if (fs == null)
                return new NotFoundResult();
            else
                return new FileStreamResult(fs, "application/octet-stream");
        }

        [HttpGet("folder/{id}/download")]
        public async Task<IActionResult> DownloadFolder(string id) //Done
        {
            AppUser user = await this.GetUser(userManager, dbContext);
            MemoryStream archive = null;

            CloudFolder folder = GetFolderFromId(id, dbContext);

            List<CloudFile> toDownload = new List<CloudFile>();

            if (folder != null) {
                if (!CanAccessFolder(folder, user, dbContext))
                    return new ForbidResult();

                foreach (CloudFile file in folder.Files)
                    toDownload.Add(file);
            } else { //Root folder
                if (user == null) //Check if the user is connected.
                    return new ForbidResult();

                toDownload.AddRange(GetFilesInRootFolder(user, dbContext));
            }

            using (var zipArchive = new ZipArchive(archive, ZipArchiveMode.Create, true)) {
                foreach (CloudFile file in toDownload) {
                    zipArchive.CreateEntryFromFile(FileSystemMiddleman.GetAbsolutePath(file), file.FileName, CompressionLevel.Fastest);
                }
            }

            return new FileStreamResult(archive, "application/zip");
        }

        [HttpPost("file/upload")]
        public async Task<IActionResult> UploadFileAsync([FromForm] UploadFileModel model)
        {
            AppUser user = await this.GetUser(userManager, dbContext);

            if (user == null) //Check if the user is connected.
                return new ForbidResult();

            CloudFolder parent = GetFolderFromId(model.FolderId, dbContext);
            if (model.FolderId != "root" && !CanAccessFolder(parent, user, dbContext))
                return new ForbidResult();

            long requestSize = 0;

            if (model.Files == null)
                return new BadRequestResult();

            foreach (IFormFile file in model.Files) {
                requestSize += file.Length;

                if (requestSize > user.AccountPlan.FileTransferSize)
                    return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.FAIL, Body = "REQUEST_MAX_TOTAL_SIZE_REACHED" }) { StatusCode = StatusCodes.Status400BadRequest };

                if (file.Length > user.AccountPlan.FileSizeLimit) //File too big
                    return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.OK, Body = "FILE_MAX_SIZE_REACHED" }) { StatusCode = StatusCodes.Status400BadRequest };

                string filename = WebUtility.HtmlEncode(file.FileName);

                CloudFile dbFile = new CloudFile()
                {
                    FileName = filename,
                    FileNameWithoutExt = Path.GetFileNameWithoutExtension(filename),
                    FileExtension = Path.GetExtension(filename),
                    FileSize = file.Length,
                    IsPublic = false,
                    OwnerId = user.Id,
                };

                if (parent != null)
                    dbFile.ParentId = parent.FolderId;

                await dbContext.Files.AddAsync(dbFile);
                //await dbContext.SaveChangesAsync();

                await FileSystemMiddleman.SaveFile(file.OpenReadStream(), user, dbFile);
            }

            await dbContext.SaveChangesAsync();

            return new JsonResult(new ResponseModel() { Status = ResponseModel.Status_.OK, Body = "FILE_SAVED" });
        }

        [NonAction]
        public List<CloudFile> GetFilesInRootFolder(AppUser owner, ApplicationDbContext dbContext)
        {
            return dbContext.Files
                .Include(f => f.Owner)
                .Include(f => f.Parent)
                .Where(x => x.Parent == null && x.Owner == owner).ToList();
        }

        [NonAction]
        public List<CloudFolder> GetFoldersInRootFolder(AppUser owner, ApplicationDbContext dbContext)
        {
            return dbContext.Folders
                .Include(f => f.Owner)
                .Include(f => f.Parent)
                .Include(f => f.Files)
                .Include(f => f.Childs)
                .Where(x => x.Parent == null && x.Owner == owner).ToList();
        }

        [NonAction]
        public List<CloudFile> GetFilesFromParent(CloudFolder parent, ApplicationDbContext dbContext)
        {
            return dbContext.Files.Where(x => x.Parent == parent).ToList();
        }

        [NonAction]
        public CloudFolder GetFolderFromId(string id, ApplicationDbContext dbContext)
        {
            if (id == "root")
                return null;
            else
                return dbContext.Folders
                    .Include(f => f.Owner)
                    .Include(f => f.Parent)
                    .Include(f => f.Files)
                    .Include(f => f.Childs)
                    .SingleOrDefault(x => x.FolderId == Guid.Parse(id));
        }

        [NonAction]
        public List<CloudFolder> GetFoldersFromId(string id, ApplicationDbContext dbContext)
        {
            if (id == "root")
                return dbContext.Folders
                    .Include(f => f.Owner)
                    .Include(f => f.Parent)
                    .Include(f => f.Files)
                    .Include(f => f.Childs)
                    .Where(x => x.Parent == null).ToList();
            else
                return dbContext.Folders
                    .Include(f => f.Owner)
                    .Include(f => f.Parent)
                    .Include(f => f.Files)
                    .Include(f => f.Childs)
                    .Where(x => x.ParentId == Guid.Parse(id)).ToList();
        }

        [NonAction]
        public JsonCloudFile GetFileAsJson(CloudFile file)
        {
            var jsonFile = new JsonCloudFile
            {
                ElementId = file.FileId.ToString(),
                FileName = file.FileName,
                FileNameWithoutExt = file.FileNameWithoutExt,
                FileInfo = $"{file.FileExtension.ToUpper()} file - {file.FileSize} MB", //TODO: Create a file type guesser function.
                CreationDate = file.CreationDate,
                LastEditDate = file.LastEditDate,
                IsPublic = file.IsPublic,
                FileSize = file.FileSize,
                OwnerId = file.OwnerId.ToString(),
                ParentId = file.ParentId.ToString(),

                /*DirectUrl = Url.Action(nameof(CloudController.File), nameof(CloudController),  new { id = file.FileId }),
                DownloadUrl = Url.Action(nameof(DownloadFile), nameof(ApiCloudController), new { id = file.FileId }),
                PreviewUrl = Url.Action(nameof(GetPreviewImage), nameof(ApiCloudController),  new { id = file.FileId }),*/
                DirectUrl = $"/My-Cloud/File/{file.FileId}",
                DownloadUrl = $"/Download/{file.FileId}",
                PreviewUrl = $"/api/v1/cloud/file/{file.FileId}/preview"
            };

            return jsonFile;
        }

        [NonAction]
        public JsonCloudFolder GetFolderAsJson(CloudFolder folder)
        {
            return new JsonCloudFolder
            {
                ElementId = folder.FolderId.ToString(),
                FolderName = folder.FolderName,
                FolderInfo = $"{folder.Files.Count} files. {folder.Childs.Count} folders.",
                CreationDate = folder.CreationDate,
                IsPublic = folder.IsPublic,
                OwnerId = folder.OwnerId.ToString(),
                ParentId = folder.ParentId.ToString(),

                DirectUrl = Url.Action(nameof(CloudController), nameof(CloudController.Folder), folder.FolderId),
                DownloadUrl = Url.Action(nameof(DownloadFolder), nameof(ApiCloudController), folder.FolderId),
            };
        }

        [NonAction]
        public JsonCloudFolder GetRootFolderAsJson(AppUser user)
        {
            return new JsonCloudFolder
            {
                ElementId = "root",
                FolderName = "Root",
                FolderInfo = "The root folder. Cannot be made public.",
                IsPublic = false,
                OwnerId = user.Id.ToString(),
                ParentId = string.Empty,

                DirectUrl = Url.Action(nameof(CloudController), nameof(CloudController.Folder), "root"),
                DownloadUrl = Url.Action(nameof(DownloadFolder), nameof(ApiCloudController), "root"),
            };
        }

        [NonAction]
        public bool CanAccessFile(CloudFile file, AppUser user, ApplicationDbContext dbContext)
        {
            if (file.IsPublic)
                return true;

            if (user == null)
                return false;

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

        [NonAction]
        public bool CanAccessFolder(CloudFolder folder, AppUser user, ApplicationDbContext dbContext)
        {
            if (folder.IsPublic)
                return true;

            if (user == null)
                return false;

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