﻿//TODO: Add a card-footer to show status (File downloaded, etc)

import * as jquery from "jquery";

$(document).ready(function () {
    $("#debug").click(function () {
        console.log("debug clicked");

        let file = new FileUIElement();
        file.ElementId = "3B07DD9A-DC09-48E2-B304-E328B9F2AD88";
        file.FileName = "test-file.png";
        file.FileInfo = "PNG Image - 5KB";
        //file.PreviewUrl = "/api/v1/cloud/preview/" + file.ElementId;
        file.PreviewUrl = "/images/no_preview.png";

        addFileUIElementToMain(file, true);
    });
});

function addFileUIElementToMain(element: FileUIElement, append: boolean) {
    let template = $("#file-template").html();

    let row = $(template);

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
        .click(() => DownloadFile(element));

    row.find("a[id='file-%id%-more']")
        .attr("id", "file-" + element.ElementId + "-more")

    if (append)
        $("#file-container").append(row);
    else
        $("#file-container").prepend(row);
}

function DownloadFile(file: FileUIElement) {
    console.log("Download clicked (FileId: " + file.ElementId + ")")
}


class FileUIElement {
    ElementId: string; //GUID
    FileName: string;
    FileInfo: string; //E.g: PNG File - 5KB
    DirectUrl: string;
    DownloadUrl: string;
    PreviewUrl: string;
}

class FolderUIElement {
    ElementId: string;
    FileName: string;
    FileInfo: string;
    DirectUrl: string;
    DownloadUrl: string;
    PreviewUrl: string;
}