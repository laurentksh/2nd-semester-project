// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

export abstract class SiteConsts {
    
}

export abstract class SiteUtil {
    public static PreloadImage(url: string) {
        const img = new Image();
        img.src = url;
        img.onload = onload;
    }
}