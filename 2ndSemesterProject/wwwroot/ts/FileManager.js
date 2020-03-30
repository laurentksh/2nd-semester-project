//TODO: Add a card-footer to show status (File downloaded, etc)
import { SiteUtil } from "./Main.js";
// Consts
const ImgPreviewUrl = "/images/no_preview.png";
//Class definitions
class FileManager {
    AddFileUIElementToMain(element, append) {
        const template = $("#file-template").html();
        const row = $(template);
        row.find("div[data-file-id]")
            .attr("data-file-id", element.ElementId);
        row.find("img[id='file-%id%-imgpreview']")
            .attr("id", "file-" + element.ElementId + "-imgpreview")
            .attr("src", element.PreviewUrl)
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
    }
    AddFolderUIElementToMain(element, append) {
    }
    DownloadFile(file) {
        console.log("Download clicked (FileId: " + file.ElementId + ")");
    }
}
class FileUIElement {
}
class FolderUIElement {
}
//Main
const fm = new FileManager();
SiteUtil.PreloadImage(ImgPreviewUrl);
$(document).ready(function () {
    $("#debug").click(() => {
        console.log("debug clicked");
        const file = new FileUIElement();
        file.ElementId = "3B07DD9A-DC09-48E2-B304-E328B9F2AD88";
        file.FileName = "test-file.png";
        file.FileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.PreviewUrl = ImgPreviewUrl;
        fm.AddFileUIElementToMain(file, true);
    });
});
//# sourceMappingURL=FileManager.js.map