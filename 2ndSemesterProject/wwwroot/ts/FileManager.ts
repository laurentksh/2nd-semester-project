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

//Classes' properties must be Camel Case
class CloudFile {
    elementId: string; //GUID
    fileName: string;
    fileNameWithoutExt: string;
    fileInfo: string; //E.g: PNG File - 5KB
    isPublic: boolean;
    creationDate: Date | string;
    lastEditDate: Date | string;
    fileSize: number;
    parentId: string;
    ownerId: string;

    directUrl: string;
    downloadUrl: string;
    previewUrl: string;

    constructor(id: string) {
        if (id === null)
            throw new TypeError("id must be a string and non-null.")

        this.elementId = id;
    }
}

class CloudFolder {
    elementId: string; //GUID
    folderName: string;
    folderInfo: string; //E.g: Folder - 36MB
    isPublic: boolean;
    creationDate: Date | string;
    parentId: string;
    ownerId: string;

    directUrl: string;
    downloadUrl: string;

    /**
     * 
     * @param id Null for root folder.
     */
    constructor(id: string | null) {
        if (id === null)
            this.elementId = "root"
        else
            this.elementId = id;
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
        const apiPromise = new Promise<unknown>((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFolders, parent.elementId), resolve, reject));

        const folders: Array<CloudFolder> = new Array<CloudFolder>()

        for (const f of await apiPromise as Array<unknown>) {
            const f2: CloudFolder = new CloudFolder(null);
            Object.assign(f2, f)
            folders.push(f2)
        }

        return folders;
    }

    public async GetChildFiles(parent: CloudFolder): Promise<Array<CloudFile>> {
        const apiPromise = new Promise<unknown>((resolve, reject) => $.getJSON(this.GetUrl(ApiUrls.ChildFiles, parent.elementId), resolve, reject));

        const files: Array<CloudFile> = new Array<CloudFile>()

        for (const f of await apiPromise as Array<unknown>) {
            const f2: CloudFile = new CloudFile("");
            Object.assign(f2, f)
            files.push(f2)
        }

        return files
    }

    public async SendFile(files: FileList, parent: CloudFolder): Promise<void> {
        const data = new FormData();

        data.append("ParentFolder", parent.elementId)

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
        console.log("Download clicked (FileId: " + file.elementId + ")")

        window.location.href = file.downloadUrl;
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
            .attr("id", `file-${element.elementId}-more`)

        if (append)
            $("#file-container").append(row);
        else
            $("#file-container").prepend(row);

        if (element.previewUrl === null || element.previewUrl === "undefined")
            DisplayImage(ImgFailedLoadingUrl, previewImg as JQuery<HTMLImageElement>, true);
        else
            DisplayImage(element.previewUrl, previewImg as JQuery<HTMLImageElement>, true);
    }

    public AddFolderToMain(element: CloudFolder, append: boolean) {
        const template = $("#folder-template").html();
        const row = $(template);

        row.find("div[data-folder-id]")
            .attr("data-folder-id", element.elementId);

        row.find("h3").text(element.folderName);
        row.find("h4").text(element.folderInfo);

        if (append)
            $("#folder-container").append(row);
        else
            $("#folder-container").prepend(row)
    }

    public AddFolderToSidebar(element: CloudFolder, append: boolean) {
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
        file.fileName = "test-file.png " + i++;
        file.fileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.previewUrl = ImgPreviewUrl;

        fm.AddFileToMain(file, true);

        const folder = new CloudFolder(null); //root

        fm.AddFolderToSidebar(folder, true);
    });
});