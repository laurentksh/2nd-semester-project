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
class CloudFile {
    constructor(id) {
        if (id === null)
            //Throw exception
            this.ElementId = id;
    }
}
class CloudFolder {
    constructor(id) {
        this.ElementId = id;
    }
}
var ApiUrls;
(function (ApiUrls) {
    ApiUrls["ChildFiles"] = "file/{0}/childs/";
    ApiUrls["ChildFolders"] = "folder/{0}/childs/";
    ApiUrls["DownloadFile"] = "file/{0}/download/";
    ApiUrls["DownloadFolder"] = "folder/{0}/download/";
    ApiUrls["FilePreview"] = "file/{0}/preview/";
    ApiUrls["FileUpload"] = "file/upload/";
})(ApiUrls || (ApiUrls = {}));
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
            const apiPromise = new Promise((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFolders, parent.ElementId), resolve, reject));
            const folders = Object.setPrototypeOf(apiPromise, CloudFolder.prototype);
            return folders;
        });
    }
    GetChildFiles(folder) {
        return __awaiter(this, void 0, void 0, function* () {
            const apiPromise = new Promise((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFiles, folder.ElementId), resolve, reject));
            const files = Object.setPrototypeOf(apiPromise, CloudFile.prototype);
            return files;
        });
    }
    SendFile(files) {
        return __awaiter(this, void 0, void 0, function* () {
            const data = new FormData();
            for (const file of files) {
                data.append("file-" + file.name, file, file.name);
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
                }
            });
        });
    }
}
ApiInterface.ApiVersion = "v1";
ApiInterface.ApiEndpoint = "api/" + ApiInterface.ApiVersion + "/";
class FileManager {
    constructor() {
        this.Api = new ApiInterface();
        //Initialize the FileManager with the current file id.
        //https://localhost/My-Cloud/File/{id}
        let folder = null;
        if (globalThis.Current !== null) {
            folder = new CloudFolder(globalThis.Current);
        }
        else {
            document.URL.split("/Folder/")[1];
        }
        if (folder !== null)
            this.Initialize(folder);
        else { //Could not determine current folder id
            alert("Could not determine folder id. Please reload the page.");
        }
    }
    Initialize(FolderId) {
        let folder = null;
        if (typeof (FolderId) !== typeof (CloudFolder)) {
            folder = new CloudFolder(FolderId);
        }
        else
            folder = FolderId;
        this.LoadFolder(folder);
    }
    SendFiles(files) {
        return this.Api.SendFile(files);
    }
    LoadFolder(folder) {
        return __awaiter(this, void 0, void 0, function* () {
            this.ClearMain(); //Clear all curent folders and files
            //Load folders first
            for (const folder_ of yield this.Api.GetChildFolders(folder)) {
                this.AddFolderToMain(folder_, true);
            }
            //Load files
            for (const file_ of yield this.Api.GetChildFiles(folder)) {
                this.AddFileToMain(file_, true);
            }
            //Displays in which folder the user is currently in
            $("#fm-sb-folders").find("div[data-folder-id'" + folder.ElementId + "'").addClass("fm-sb-folders-current");
        });
    }
    LoadSideBar() {
        return __awaiter(this, void 0, void 0, function* () {
            for (const folder of yield this.Api.GetChildFolders(null)) {
                this.AddFolderToSidebar(folder, true); //Add another LoadSidebar call on Click event
            }
        });
    }
    AddFileToMain(element, append) {
        const template = $("#file-template").html();
        const row = $(template);
        row.find("div[data-file-id]")
            .attr("data-file-id", element.ElementId);
        const previewImg = row.find("img[id='file-%id%-imgpreview']");
        previewImg.attr("id", "file-" + element.ElementId + "-imgpreview")
            .attr("alt", element.FileName + " - Preview");
        row.find("h5").text(element.FileName);
        row.find("h6").text(element.FileInfo);
        row.find("a[id='file-%id%-download']")
            .attr("id", "file-" + element.ElementId + "-download")
            .click(() => this.DownloadFile(element));
        row.find("a[id='file-%id%-more']")
            .attr("id", "file-" + element.ElementId + "-more");
        if (append)
            $("#file-container").append(row);
        else
            $("#file-container").prepend(row);
        DisplayImage(element.PreviewUrl, previewImg, true);
    }
    AddFolderToMain(element, append) {
        console.log(element.ElementId + " " + append);
    }
    AddFolderToSidebar(element, append) {
        const template = $("#fm-sb-folder-template").html();
        const row = $(template);
        row.find("div[data-folder-id='%id%']")
            .attr("data-file-id", element.ElementId);
        row.find("p").text(element.FolderName);
        if (append)
            $("#fm-sb-folders").append(row);
        else
            $("#fm-sb-folders").prepend(row);
    }
    DownloadFile(file) {
        console.log("Download clicked (FileId: " + file.ElementId + ")");
        document.open(file.DirectUrl, file.FileName);
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
    const input = fc.find('#fm-input');
    fc.on("dragover", (event) => {
        event.preventDefault();
        event.stopPropagation();
        $(this).addClass('fm-dragging');
    });
    fc.on("dragleave", (event) => {
        event.preventDefault();
        event.stopPropagation();
        $(this).removeClass('fm-dragging');
    });
    fc.on("drop", (event) => {
        event.preventDefault();
        event.stopPropagation();
        fm.SendFiles(event.originalEvent.dataTransfer.files);
    });
    input.on("change", (event) => {
        console.log("change: " + event);
    });
    $("#debug").click(() => {
        console.log("debug clicked");
        const file = new CloudFile("3B07DD9A-DC09-48E2-B304-E328B9F2AD88");
        file.FileName = "test-file.png " + i++;
        file.FileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.PreviewUrl = ImgPreviewUrl;
        fm.AddFileToMain(file, true);
        const folder = new CloudFolder(null);
        folder.FolderName = "abc";
        fm.AddFolderToSidebar(folder, true);
    });
});
//# sourceMappingURL=FileManager.js.map