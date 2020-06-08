//TODO: Add a card-footer to show status (File downloaded, etc)
//TODO: To fix AzPipelines, try adding an import statement here for @types/jquery
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
// Consts
const ImgPreviewUrl = "/images/no_preview.png";
const ImgLoadingPreviewUrl = "/images/loading.svg";
const ImgFailedLoadingUrl = "/images/failed_loading.png";
//Class definitions
function PreloadImage(url) {
    const img = new Image();
    img.src = url;
    img.onload = null;
}
function DisplayImage(url, element, showIcons = true) {
    const img = new Image();
    img.onload = () => {
        console.log("Loaded " + img.src);
        element.attr("src", img.src);
    };
    if (showIcons) {
        element.attr("src", ImgLoadingPreviewUrl);
        img.onerror = () => {
            console.error("Failed loading the following image: " + img.src);
            element.attr("src", ImgFailedLoadingUrl);
        };
    }
    console.log("Loading " + img.src);
    img.src = url;
}
//Classes' properties must be Camel Case
class CloudFile {
    constructor(id) {
        if (id === null)
            throw new TypeError("id must be a string and non-null.");
        this.elementId = id;
    }
}
class CloudFolder {
    /**
     *
     * @param id Null for root folder.
     */
    constructor(id) {
        if (id === null)
            this.elementId = "root";
        else
            this.elementId = id;
    }
}
class JsonFolderSharedAccess {
}
class JsonFileSharedAccess {
}
var ApiUrls;
(function (ApiUrls) {
    ApiUrls["GetFile"] = "file/{0}";
    ApiUrls["GetFolder"] = "folder/{0}";
    ApiUrls["ChildFiles"] = "file/{0}/childs/";
    ApiUrls["ChildFolders"] = "folder/{0}/childs/";
    ApiUrls["DownloadFile"] = "file/{0}/download/";
    ApiUrls["DownloadFolder"] = "folder/{0}/download/";
    ApiUrls["FilePreview"] = "file/{0}/preview/";
    ApiUrls["FileUpload"] = "file/upload/";
})(ApiUrls || (ApiUrls = {}));
var AlertStyle;
(function (AlertStyle) {
    AlertStyle["Primary"] = "alert-primary";
    AlertStyle["Secondary"] = "alert-secondary";
    AlertStyle["Success"] = "alert-success";
    AlertStyle["Danger"] = "alert-danger";
    AlertStyle["Warning"] = "alert-warning";
    AlertStyle["Info"] = "alert-info";
    AlertStyle["Light"] = "alert-light";
    AlertStyle["Dark"] = "alert-dark";
})(AlertStyle || (AlertStyle = {}));
let ApiInterface = /** @class */ (() => {
    class ApiInterface {
        constructor() {
            //Perhaps initialize an API token ?
        }
        GetUrl(url, id) {
            if (id === null)
                return ApiInterface.ApiEndpoint + url;
            else
                return ApiInterface.ApiEndpoint + url.replace("{0}", id);
        }
        GetChildFolders(parent) {
            return __awaiter(this, void 0, void 0, function* () {
                const apiPromise = new Promise((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFolders, parent.elementId), resolve, reject));
                const folders = new Array();
                for (const f of yield apiPromise) {
                    const f2 = new CloudFolder(null);
                    Object.assign(f2, f);
                    folders.push(f2);
                }
                return folders;
            });
        }
        GetChildFiles(parent) {
            return __awaiter(this, void 0, void 0, function* () {
                const apiPromise = new Promise((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFiles, parent.elementId), resolve, reject));
                const files = new Array();
                for (const f of yield apiPromise) {
                    const f2 = new CloudFile("");
                    Object.assign(f2, f);
                    files.push(f2);
                }
                return files;
            });
        }
        SendFile(files, parent) {
            return __awaiter(this, void 0, void 0, function* () {
                const data = new FormData();
                data.append("ParentFolder", parent.elementId);
                for (const file of files) {
                    data.append("File", file, file.name);
                }
                $.ajax({
                    url: this.GetUrl(ApiUrls.FileUpload, null),
                    data: data,
                    cache: false,
                    contentType: false,
                    processData: false,
                    method: "POST",
                    success: () => {
                        //Display a sneaky alert to tell the user his files sent correctly.
                    },
                    error: () => {
                        //Display a sneaky alert to tell the user his files did not sent correctly.
                        alert("An error occured while sending the file(s).");
                    }
                });
            });
        }
    }
    ApiInterface.ApiVersion = "v1";
    ApiInterface.ApiEndpoint = "api/" + ApiInterface.ApiVersion + "/cloud/";
    return ApiInterface;
})();
class FileManager {
    constructor() {
        this.Api = new ApiInterface();
        //Initialize the FileManager with the current file id.
        //https://localhost/My-Cloud/File/{id}
        let folder = null;
        if (globalThis.Current !== undefined) {
            if (globalThis.Current === "root")
                folder = new CloudFolder(null);
            else
                folder = new CloudFolder(globalThis.Current);
        }
        else { //Last resort, shouldn't really happen but you're never too sure ¯\_(ツ)_/¯
            document.URL.split("/Folder/")[1].replace("/", "");
        }
        if (folder !== null)
            this.Initialize(folder);
        else { //Could not determine current folder id
            alert("Could not determine folder id. Please reload the page.");
            window.location.href = "/";
        }
    }
    /**
     * Initialize the FileManager
     * @param Folder
     */
    Initialize(Folder) {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.LoadFolder(Folder !== null && Folder !== void 0 ? Folder : new CloudFolder(null));
        });
    }
    /**
     * Load a folder as the current folder.
     *
     * Calling this method alone will do the following:
     * - Clear main and the sidebar
     * - Load all folders in main and in the sidebar
     * - Load all files
     * - Set the current folder as the one provided
     *
     * @param folder Folder to load
     *
     */
    LoadFolder(folder) {
        return __awaiter(this, void 0, void 0, function* () {
            this.ClearMain(); //Clear all current folders and files
            this.ClearSideBar(); //Clear the sidebar
            const childFolders = yield this.Api.GetChildFolders(folder);
            const childFiles = yield this.Api.GetChildFiles(folder);
            let folderParseErrorCount = 0;
            let fileParseErrorCount = 0;
            //Load folders first
            for (const folder_ of childFolders) {
                if (folder_ instanceof CloudFolder === false) {
                    folderParseErrorCount++;
                    continue;
                }
                this.AddFolderToMain(folder_, true);
                this.AddFolderToSidebar(folder_, true);
            }
            //Load files
            for (const file_ of childFiles) {
                if (file_ instanceof CloudFile === false) {
                    fileParseErrorCount++;
                    continue;
                }
                this.AddFileToMain(file_, true);
            }
            if (folderParseErrorCount !== 0)
                this.DisplayMessage(`An error occured while parsing the folder. (${folderParseErrorCount}x)`, AlertStyle.Danger);
            if (fileParseErrorCount !== 0)
                this.DisplayMessage(`An error occured while parsing the file. (${fileParseErrorCount}x)`, AlertStyle.Danger);
            this.Current = folder;
            if (childFolders.length !== 0) {
                //Displays in which folder the user is currently in
                $("#fm-sb-folders").find(`div[data-folder-id'${folder.elementId}']`).addClass("fm-sb-folders-current"); //undefined is not a function
            }
            if (childFiles.length !== 0) {
                //Do nothing
            }
            if (childFiles.length === 0 && childFolders.length === 0) { //If there's nothing in the folder.
                if (folder.elementId === "root")
                    this.DisplayMessage("Welcome ! This is your root folder. Drag some files here to upload them or create a folder by clicking the + button.", AlertStyle.Success);
                else
                    this.DisplayMessage("This folder is empty.", AlertStyle.Info);
            }
        });
    }
    LoadSideBar(folderId) {
        return __awaiter(this, void 0, void 0, function* () {
            for (const folder of yield this.Api.GetChildFolders(folderId)) {
                this.AddFolderToSidebar(folder, true); //Add another LoadSidebar call on Click event
            }
        });
    }
    SendFiles(files, folder) {
        return __awaiter(this, void 0, void 0, function* () {
            return yield this.Api.SendFile(files, folder !== null && folder !== void 0 ? folder : this.Current);
        });
    }
    DownloadFile(file) {
        console.log("Download clicked (FileId: " + file.elementId + ")");
        window.location.href = file.directUrl;
    }
    // UI methods
    DisplayMessage(message, alertStyle) {
        $("#file-container").append(`<div class="alert ${alertStyle} msg-alert" role="alert">${message}</div>`);
    }
    HideMessage() {
        $("#file-container").find(".msg-alert").remove();
    }
    AddFileToMain(element, append) {
        const template = $("#file-template").html();
        const row = $(template);
        row.find("div[data-file-id]")
            .attr("data-file-id", element.elementId);
        const previewImg = row.find("img[id='file-%id%-imgpreview']");
        previewImg.attr("id", `file-${element.elementId}-imgpreview`)
            .attr("alt", element.fileName + " - Preview");
        row.find("h5").text(element.fileName);
        row.find("h6").text(element.fileInfo);
        row.find("a[id='file-%id%-download']")
            .attr("id", `file-${element.elementId}-download`)
            .click(() => this.DownloadFile(element));
        row.find("a[id='file-%id%-more']")
            .attr("id", `file-${element.elementId}-more`);
        if (append)
            $("#file-container").append(row);
        else
            $("#file-container").prepend(row);
        if (element.previewUrl === null || element.previewUrl === "undefined")
            DisplayImage(ImgFailedLoadingUrl, previewImg, true);
        else
            DisplayImage(element.previewUrl, previewImg, true);
    }
    AddFolderToMain(element, append) {
        //TODO
    }
    AddFolderToSidebar(element, append) {
        const template = $("#fm-sb-folder-template").html();
        const row = $(template);
        row.find("div[data-folder-id='%id%']")
            .attr("data-folder-id", element.elementId);
        row.find("p").text(element.folderName);
        row.click(() => this.LoadFolder(element));
        this.AddUploadEvents(row, element);
        if (append)
            $("#fm-sb-folders").append(row);
        else
            $("#fm-sb-folders").prepend(row);
    }
    AddUploadEvents(elem, folder) {
        elem.on("dragover", (event) => {
            event.preventDefault();
            event.stopPropagation();
            elem.addClass('fm-dragging');
        });
        elem.on("dragleave", (event) => {
            event.preventDefault();
            event.stopPropagation();
            elem.removeClass('fm-dragging');
        });
        elem.on("drop", (event) => {
            event.preventDefault();
            event.stopPropagation();
            elem.removeClass('fm-dragging');
            this.SendFiles(event.originalEvent.dataTransfer.files, folder);
        });
    }
    ClearMain() {
        $("#file-container").empty();
    }
    ClearSideBar() {
        $("#fm-sb-folders").empty();
    }
}
//Main
const fm = new FileManager();
PreloadImage(ImgPreviewUrl);
PreloadImage(ImgFailedLoadingUrl);
PreloadImage(ImgLoadingPreviewUrl);
let i = 0;
//https://stackoverflow.com/questions/8363464/jquery-drag-n-drop-files-how-to-get-file-info
$(document).ready(() => {
    const fc = $(".fm-container");
    fm.AddUploadEvents(fc, null);
    $("#debug").click(() => {
        console.log("debug clicked");
        const file = new CloudFile("3B07DD9A-DC09-48E2-B304-E328B9F2AD" + i);
        file.fileName = "test-file.png " + i++;
        file.fileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.previewUrl = ImgPreviewUrl;
        fm.AddFileToMain(file, true);
        const folder = new CloudFolder(null); //root
        fm.AddFolderToSidebar(folder, true);
    });
});
//# sourceMappingURL=FileManager.js.map