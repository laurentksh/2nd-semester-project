﻿using System;
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

namespace _2ndSemesterProject.Controllers.Api.v1
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/cloud")]
    public class CloudController : ControllerBase
    {
        [HttpGet("file/{id}")]
        public IActionResult GetFile(Guid id)
        {
            JsonCloudFile fileJson = null;

            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            //Boilerplate code for checking if the user is allowed to download the file
            if (!User.Identity.IsAuthenticated)
                return new UnauthorizedResult();

            if (User.Identity.IsAuthenticated) {
                user = this.GetUser();
            }

            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (!file.IsPublic) { //Check if the current user has the permission to download the file
                FolderSharedAccess fsa = dbContext.FolderSharedAccesses
                    .Where(a => a.ReceiverId == user.Id)
                    .Where(a2 => a2.SharedFolderId == file.ParentId)
                    .FirstOrDefault();

                FileSharedAccess fsa2 = dbContext.FileSharedAccesses
                    .Where(b => b.ReceiverId == user.Id)
                    .Where(b2 => b2.SharedFileId == file.FileId)
                    .FirstOrDefault();

                if (fsa == default(FolderSharedAccess) && fsa2 == default(FileSharedAccess))
                    return new UnauthorizedResult();
            }

            fileJson = new JsonCloudFile
            {
                ElementId = file.FileId.ToString(),
                FileName = file.FileNameWithoutExt,
                //TODO: Create a file type guesser function.
                FileInfo = $"{file.FileExtension.ToUpper()} file - {file.FileSize} MB",
                DirectUrl = Url.Action(nameof(Controllers.CloudController), nameof(Controllers.CloudController.File), file.FileId),
                DownloadUrl = Url.Action(nameof(DownloadFile), nameof(CloudController), file.FileId),
                PreviewUrl = Url.Action(nameof(GetPreviewImage), nameof(CloudController), file.FileId)
            };

            return new JsonResult(file);
        }

        [HttpGet("folder/{id}")]
        public IActionResult GetFolder(Guid id)
        {
            JsonCloudFolder folder = new JsonCloudFolder();

            return new JsonResult(folder);
        }

        [HttpGet("file/{id}/preview")]
        public IActionResult GetPreviewImage(Guid id)
        {
            //TODO: Get file preview from folder (example path: "/<userId>/<fileId>-preview.jpg")
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;


            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (file.IsPublic)
                return new FileStreamResult(FileSystemMiddleman.GetFile(file, "-preview.jpg"), "image/jpeg");

            if (!User.Identity.IsAuthenticated)
                return new UnauthorizedResult();

            user = this.GetUser();

            //Check if the current user has the permission to download the file
            FolderSharedAccess fsa = dbContext.FolderSharedAccesses
                .Where(a => a.ReceiverId == user.Id)
                .Where(a2 => a2.SharedFolderId == file.ParentId)
                .FirstOrDefault();

            FileSharedAccess fsa2 = dbContext.FileSharedAccesses
                .Where(b => b.ReceiverId == user.Id)
                .Where(b2 => b2.SharedFileId == file.FileId)
                .FirstOrDefault();

            if (fsa == default(FolderSharedAccess) && fsa2 == default(FileSharedAccess))
                return new UnauthorizedResult();

            return new FileStreamResult(FileSystemMiddleman.GetFile(file, "-preview.jpg"), "image/jpeg");
        }

        [HttpGet("file/{id}/download")]
        public IActionResult DownloadFile(Guid id, bool anonymous = false)
        {
            ApplicationDbContext dbContext = new ApplicationDbContext();
            AppUser user = null;

            //Boilerplate code for checking if the user is allowed to download the file
            if (!User.Identity.IsAuthenticated && !anonymous)
                return new UnauthorizedResult();

            if (User.Identity.IsAuthenticated) {
                user = this.GetUser();
            }

            CloudFile file = dbContext.Files.Where(f => f.FileId == id).First();

            if (anonymous && !file.IsPublic) { //Check if the current user has the permission to download the file
                FolderSharedAccess fsa = dbContext.FolderSharedAccesses
                    .Where(a => a.ReceiverId == user.Id)
                    .Where(a2 => a2.SharedFolderId == file.ParentId)
                    .FirstOrDefault();

                FileSharedAccess fsa2 = dbContext.FileSharedAccesses
                    .Where(b => b.ReceiverId == user.Id)
                    .Where(b2 => b2.SharedFileId == file.FileId)
                    .FirstOrDefault();

                if (fsa == default(FolderSharedAccess) && fsa2 == default(FileSharedAccess))
                    return new UnauthorizedResult();
            }

            var fs = FileSystemMiddleman.GetFile(file);

            if (fs == null)
                return new NotFoundResult();

            return new FileStreamResult(fs, "application/octet-stream");
        }

        [HttpGet("folder/{id}/download")]
        public IActionResult DownloadFolder(Guid id)
        {
            //TODO

            return new FileStreamResult(null, "application/zip");
        }
    }
}