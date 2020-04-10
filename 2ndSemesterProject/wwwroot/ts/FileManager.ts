//TODO: Add a card-footer to show status (File downloaded, etc)
//TODO: To fix AzPipelines, try adding an import statement here for @types/jquery


// Consts

const ImgPreviewUrl = "/images/no_preview.png";
const ImgLoadingPreviewUrl = "/images/loading.svg";
const ImgFailedLoadingUrl = "/images/failed_loading.png";


//Class definitions

function PreloadImage(url: string) {
    const img = new Image();
    img.src = url;
    img.onload = null;
}

function DisplayImage(url: string, element: JQuery<HTMLImageElement>, showIcons = true) {
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


    console.log("Loading " + img.src)
    img.src = url;
}

enum ApiUrls {
    ChildFiles = "file/{0}/childs/",
    ChildFolders = "folder/{0}/childs/",
    DownloadFile = "file/{0}/download/",
    DownloadFolder = "folder/{0}/download/",
    FilePreview = "file/{0}/preview/"
}

class ApiInterface {
    public static readonly ApiVersion = "v1";
    public static readonly ApiEndpoint = "api/" + ApiInterface.ApiVersion + "/";

    //Solution: Use a callback
    public async GetChildFolders(parent: CloudFolder | null): Promise<Array<CloudFolder>> {
        //TODO

        const apiPromise = new Promise<any>((resolve, reject) => $.getJSON(ApiInterface.ApiEndpoint + ApiUrls.ChildFolders.replace("{0}", parent.ElementId), resolve, reject));

        await apiPromise.then();

        return null;
    }

    public async GetChildFiles(folder: CloudFolder | null): Promise<Array<CloudFile>> {

        return null;
    }
}

class FileManager {
    private Api: ApiInterface;

    constructor() {
        this.Api = new ApiInterface();
    }

    public async LoadFolder(folder: CloudFolder) {
        this.ClearMain(); //Clear all curent folders and files

        //Load folders first


        for (const folder_ of await this.Api.GetChildFolders(folder)) {
            this.AddFolderToMain(folder_, true);
        }

        //Load files
        for (const file_ of await this.Api.GetChildFiles(folder)) {
            this.AddFileToMain(file_, true);
        }

        //Displays in which folder the user is currently in
        $("#fm-sb-folders").find("div[data-folder-id'" + folder.ElementId + "'")
            .addClass("fm-sb-folders-current");
    }

    public async LoadSideBar() {
        for (const folder of await this.Api.GetChildFolders(null)) {
            this.AddFolderToSidebar(folder, true); //Add another LoadSidebar call on Click event
        }
    }

    public AddFileToMain(element: CloudFile, append: boolean) {
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
            .attr("id", "file-" + element.ElementId + "-more")

        if (append)
            $("#file-container").append(row);
        else
            $("#file-container").prepend(row);

        DisplayImage(element.PreviewUrl, previewImg as JQuery<HTMLImageElement>, true);
    }

    public AddFolderToMain(element: CloudFolder, append: boolean) {
        console.log(element.ElementId + " " + append);
    }

    public AddFolderToSidebar(element: CloudFolder, append: boolean) {
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

    public DownloadFile(file: CloudFile) {
        console.log("Download clicked (FileId: " + file.ElementId + ")")

        document.open(file.DirectUrl, file.FileName);
    }

    public ClearMain() {
        $("#file-container").empty();
    }

    public ClearSideBar() {
        $("#fm-sb-folders").empty();
    }
}

class CloudFile {
    ElementId: string; //GUID
    FileName: string; //Without file ext
    FileInfo: string; //E.g: PNG File - 5KB
    DirectUrl: string;
    DownloadUrl: string;
    PreviewUrl: string;
}

class CloudFolder {
    ElementId: string; //GUID
    FolderName: string;
    FolderInfo: string; //E.g: Folder - 36MB
    DirectUrl: string;
    DownloadUrl: string;
}

//Main

const fm = new FileManager();

PreloadImage(ImgPreviewUrl);
PreloadImage(ImgFailedLoadingUrl);
PreloadImage(ImgLoadingPreviewUrl);


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