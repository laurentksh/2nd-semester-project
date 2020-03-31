//TODO: Add a card-footer to show status (File downloaded, etc)
//TODO: To fix AzPipelines, try adding an import statement here for @types/jquery
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
class ApiInterface {
    GetChildFolders(inFolder, recursive) {
        //TODO
        $.getJSON(ApiInterface.ApiEndpoint, null, () => {
        });
        return null;
    }
    GetChildFiles(folder) {
        return null;
    }
}
ApiInterface.ApiVersion = "v1";
ApiInterface.ApiEndpoint = "/api/" + ApiInterface.ApiVersion + "/";
class FileManager {
    constructor() {
        this.Api = new ApiInterface();
    }
    LoadFolder(folder) {
        this.ClearMain(); //Clear all curent folders and files
        //Load folders first
        for (const folder_ of this.Api.GetChildFolders(folder)) {
            this.AddFolderToMain(folder_, true);
        }
        //Load files
        for (const file_ of this.Api.GetChildFiles(folder)) {
            this.AddFileToMain(file_, true);
        }
        //Displays in which folder the user is currently in
        $("#fm-sb-folders").find("div[data-folder-id'" + folder.ElementId + "'")
            .addClass("fm-sb-folders-current");
    }
    LoadSideBar() {
        for (let folder of this.Api.GetChildFolders(null, true)) {
            this.AddFolderToSidebar(folder, true);
        }
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
}
class CloudFile {
}
class CloudFolder {
}
//Main
const fm = new FileManager();
PreloadImage(ImgPreviewUrl);
let i = 0;
$(document).ready(function () {
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