// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
export class SiteConsts {
}
export class SiteUtil {
    static PreloadImage(url) {
        const img = new Image();
        img.src = url;
        img.onload = onload;
    }
}
//# sourceMappingURL=Main.js.map