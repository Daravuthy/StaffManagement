window.staffManagementStorage = {
    set: function (key, value) {
        localStorage.setItem(key, value);
    },
    get: function (key) {
        return localStorage.getItem(key);
    },
    remove: function (key) {
        localStorage.removeItem(key);
    }
};

window.staffManagementDownloads = {
    saveFile: function (fileName, contentType, base64Content) {
        const bytes = Uint8Array.from(atob(base64Content), c => c.charCodeAt(0));
        const blob = new Blob([bytes], { type: contentType });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement("a");
        anchor.href = url;
        anchor.download = fileName;
        anchor.click();
        anchor.remove();
        URL.revokeObjectURL(url);
    }
};
