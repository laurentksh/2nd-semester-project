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


class CloudFile {
    ElementId: string; //GUID
    FileName: string;
    FileNameWithoutExt: string;
    FileInfo: string; //E.g: PNG File - 5KB
    IsPublic: boolean;
    CreationDate: Date | string;
    LastEditDate: Date | string;
    FileSize: number;
    ParentId: string;
    OwnerId: string;

    DirectUrl: string;
    DownloadUrl: string;
    PreviewUrl: string;

    constructor(id: string) {
        if (id === null)
            throw new TypeError("id must be a string and non-null.")

        this.ElementId = id;
    }
}

class CloudFolder {
    ElementId: string; //GUID
    FolderName: string;
    FolderInfo: string; //E.g: Folder - 36MB
    IsPublic: boolean;
    CreationDate: Date | string;
    ParentId: string;
    OwnerId: string;

    DirectUrl: string;
    DownloadUrl: string;

    /**
     * 
     * @param id Null for root folder.
     */
    constructor(id: string | null) {
        if (id === null)
            this.ElementId = "root"
        else
            this.ElementId = id;
    }
}

class JsonFolderSharedAccess {
    Id: string;

    /**UTC. DateTime.MinValue: No expiration */
    ExpirationDate: Date | string;


    /**Who shared this folder with you */
    Sender: string;

    /**Who received this folder access */
    Receiver: string;

    /**Shared folder */
    SharedFolder: string;
}

class JsonFileSharedAccess {
    Id: string;

    /**UTC. DateTime.MinValue: No expiration */
    ExpirationDate: Date | string;

    /**Who shared this folder with you */
    SenderId: string;

    /**Who received this folder access */
    ReceiverId: string;

    /**Shared folder */
    SharedFileId: string;
}

enum ApiUrls {
    GetFile = "file/{0}",
    GetFolder = "folder/{0}",
    ChildFiles = "file/{0}/childs/",
    ChildFolders = "folder/{0}/childs/",
    DownloadFile = "file/{0}/download/",
    DownloadFolder = "folder/{0}/download/",
    FilePreview = "file/{0}/preview/",
    FileUpload = "file/upload/"
}

enum AlertStyle {
    Primary = "alert-primary",
    Secondary = "alert-secondary",
    Success = "alert-success",
    Danger = "alert-danger",
    Warning = "alert-warning",
    Info = "alert-info",
    Light = "alert-light",
    Dark = "alert-dark"
}

class ApiInterface {
    public static readonly ApiVersion = "v1";
    public static readonly ApiEndpoint = "api/" + ApiInterface.ApiVersion + "/cloud/";

    constructor() {
        //Perhaps initialize an API token ?
    }

    public GetUrl(url: ApiUrls, id: string | null): string {
        if (id === null)
            return ApiInterface.ApiEndpoint + url;
        else
            return ApiInterface.ApiEndpoint + url.replace("{0}", id);
    }

    public async GetChildFolders(parent: CloudFolder): Promise<Array<CloudFolder>> {
        //TODO: Parse manually the json nodes

        const apiPromise = new Promise<object>((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFolders, parent.ElementId), resolve, reject));
        //const folders: Array<CloudFolder> = Object.setPrototypeOf(await apiPromise /* Wait for the request to be completed */, CloudFolder.prototype)

        return /*folders;*/Object.assign(new Array<CloudFolder>(), await apiPromise)
    }

    public async GetChildFiles(parent: CloudFolder): Promise<Array<CloudFile>> {
        //TODO: Parse manually the json nodes

        const apiPromise = new Promise<object>((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFiles, parent.ElementId), resolve, reject));
        //const json: Array<any> = await apiPromise

        //const files: Array<CloudFile> = new Array<CloudFile>()
        /*for (const file in await apiPromise) {

        }*/

        //const files: Array<CloudFile> = Object.setPrototypeOf(await apiPromise, Array<CloudFolder>())

        return /*files;*/Object.assign(new Array<CloudFile>(), await apiPromise);
    }

    public async SendFile(files: FileList, parent: CloudFolder): Promise<void> {
        const data = new FormData();

        data.append("ParentFolder", parent.ElementId)

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
                alert("An error occured while sending the file(s).")
            }
        })
    }
}

class FileManager {
    private Api: ApiInterface;

    public Current: CloudFolder;

    constructor() {
        this.Api = new ApiInterface();

        //Initialize the FileManager with the current file id.
        //https://localhost/My-Cloud/File/{id}

        let folder = null;

        if (globalThis.Current !== undefined) {
            if (globalThis.Current === "root")
                folder = new CloudFolder(null)
            else
                folder = new CloudFolder(globalThis.Current);
        } else { //Last resort, shouldn't really happen but you're never too sure ¯\_(ツ)_/¯
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
    public async Initialize(Folder: CloudFolder | null) {
        await this.LoadFolder(Folder ?? new CloudFolder(null));
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
    public async LoadFolder(folder: CloudFolder) {
        this.ClearMain(); //Clear all current folders and files
        this.ClearSideBar(); //Clear the sidebar

        const childFolders = await this.Api.GetChildFolders(folder);
        const childFiles = await this.Api.GetChildFiles(folder);

        let folderParseErrorCount = 0;
        let fileParseErrorCount = 0;

        //Load folders first
        for (const folder_ of childFolders) {
            if (Object.getPrototypeOf(folder_) !== CloudFolder.prototype) {
                folderParseErrorCount++;
                continue;
            }

            this.AddFolderToMain(folder_, true);
            this.AddFolderToSidebar(folder_, true);
        }

        //Load files
        for (const file_ of childFiles) {
            if (Object.getPrototypeOf(file_) !== CloudFile.prototype) {
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
            $("#fm-sb-folders").find(`div[data-folder-id'${folder.ElementId}']`).addClass("fm-sb-folders-current"); //undefined is not a function
        }

        if (childFiles.length !== 0) {
            //Do nothing
        }

        if (childFiles.length === 0 && childFolders.length === 0) { //If there's nothing in the folder.
            if (folder.ElementId === "root")
                this.DisplayMessage("Welcome ! This is your root folder. Drag some files here to upload them or create a folder by clicking the + button.", AlertStyle.Success)
            else
                this.DisplayMessage("This folder is empty.", AlertStyle.Info)
        }
    }

    public async LoadSideBar(folderId: CloudFolder) {
        for (const folder of await this.Api.GetChildFolders(folderId)) {
            this.AddFolderToSidebar(folder, true); //Add another LoadSidebar call on Click event
        }
    }

    public async SendFiles(files: FileList, folder?: CloudFolder): Promise<void> {
        return await this.Api.SendFile(files, folder ?? this.Current);
    }

    public DownloadFile(file: CloudFile) {
        console.log("Download clicked (FileId: " + file.ElementId + ")")

        window.location.href = file.DirectUrl;
    }


    // UI methods

    public DisplayMessage(message: string, alertStyle: AlertStyle) {
        $("#file-container").append(`<div class="alert ${alertStyle} msg-alert" role="alert">${message}</div>`)
    }

    public HideMessage() {
        $("#file-container").find(".msg-alert").remove()
    }

    public AddFileToMain(element: CloudFile, append: boolean) {
        const template = $("#file-template").html();
        const row = $(template);

        row.find("div[data-file-id]")
            .attr("data-file-id", element.ElementId);

        const previewImg = row.find("img[id='file-%id%-imgpreview']");

        previewImg.attr("id", `file-${element.ElementId}-imgpreview`)
            .attr("alt", element.FileName + " - Preview");

        row.find("h5").text(element.FileName);
        row.find("h6").text(element.FileInfo);

        row.find("a[id='file-%id%-download']")
            .attr("id", `file-${element.ElementId}-download`)
            .click(() => this.DownloadFile(element));

        row.find("a[id='file-%id%-more']")
            .attr("id", `file-${element.ElementId}-more`)

        if (append)
            $("#file-container").append(row);
        else
            $("#file-container").prepend(row);

        if (element.PreviewUrl === null || element.PreviewUrl === "undefined")
            DisplayImage(ImgFailedLoadingUrl, previewImg as JQuery<HTMLImageElement>, true);
        else
            DisplayImage(element.PreviewUrl, previewImg as JQuery<HTMLImageElement>, true);
    }

    public AddFolderToMain(element: CloudFolder, append: boolean) {
        //TODO
    }

    public AddFolderToSidebar(element: CloudFolder, append: boolean) {
        const template = $("#fm-sb-folder-template").html();
        const row = $(template);

        row.find("div[data-folder-id='%id%']")
            .attr("data-folder-id", element.ElementId);

        row.find("p").text(element.FolderName);

        row.click(() => this.LoadFolder(element));

        this.AddUploadEvents(row, element);

        if (append)
            $("#fm-sb-folders").append(row);
        else
            $("#fm-sb-folders").prepend(row);
    }

    public AddUploadEvents(elem: JQuery<HTMLElement>, folder: CloudFolder | null) {
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

    public ClearMain() {
        $("#file-container").empty();
    }

    public ClearSideBar() {
        $("#fm-sb-folders").empty();
    }
    //End UI methods
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
        file.FileName = "test-file.png " + i++;
        file.FileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.PreviewUrl = ImgPreviewUrl;

        fm.AddFileToMain(file, true);

        const folder = new CloudFolder(null); //root

        fm.AddFolderToSidebar(folder, true);
    });
});