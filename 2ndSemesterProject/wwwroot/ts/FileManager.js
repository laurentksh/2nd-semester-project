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
var ApiUrls;
(function (ApiUrls) {
    ApiUrls["ChildFiles"] = "file/{0}/childs/";
    ApiUrls["ChildFolders"] = "folder/{0}/childs/";
    ApiUrls["DownloadFile"] = "file/{0}/download/";
    ApiUrls["DownloadFolder"] = "folder/{0}/download/";
    ApiUrls["FilePreview"] = "file/{0}/preview/";
})(ApiUrls || (ApiUrls = {}));
class CloudFile {
}
class CloudFolder {
}
class ApiInterface {
    //Solution: Use a callback
    GetChildFolders(parent) {
        return __awaiter(this, void 0, void 0, function* () {
            //TODO
            const apiPromise = new Promise((resolve, reject) => $.getJSON(ApiInterface.ApiEndpoint + ApiUrls.ChildFolders.replace("{0}", parent.ElementId), resolve, reject));
            yield apiPromise.then();
            Object.setPrototypeOf(yield apiPromise.then(), CloudFolder.prototype);
            return null;
        });
    }
    GetChildFiles(folder) {
        return __awaiter(this, void 0, void 0, function* () {
            return null;
        });
    }
}
ApiInterface.ApiVersion = "v1";
ApiInterface.ApiEndpoint = "api/" + ApiInterface.ApiVersion + "/";
class FileManager {
    constructor() {
        this.Api = new ApiInterface();
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
            $("#fm-sb-folders").find("div[data-folder-id'" + folder.ElementId + "'")
                .addClass("fm-sb-folders-current");
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
$(document).ready(() => {
    var fc = $(".fm-container");
    var input = fc.find('#fm-input');
    fc.on("dragover", (event) => {
        event.preventDefault();
        event.stopPropagation();
        $(this).addClass('dragging');
    });
    fc.on("dragleave", (event) => {
        event.preventDefault();
        event.stopPropagation();
        $(this).removeClass('dragging');
    });
    fc.on("drop", (event) => {
        event.preventDefault();
        event.stopPropagation();
    });
    input.on("change", (e) => {
        for (var file in this.files) {
        }
    });
    $("#debug").click(() => {
        console.log("debug clicked");
        const file = new CloudFile();
        file.ElementId = "3B07DD9A-DC09-48E2-B304-E328B9F2AD88";
        file.FileName = "test-file.png " + i++;
        file.FileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.PreviewUrl = ImgPreviewUrl;
        fm.AddFileToMain(file, true);
        const folder = new CloudFolder();
        folder.FolderName = "abc";
        fm.AddFolderToSidebar(folder, true);
    });
});
//# sourceMappingURL=FileManager.js.map