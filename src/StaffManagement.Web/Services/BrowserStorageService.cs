using Microsoft.JSInterop;

namespace StaffManagement.Web.Services;

public sealed class BrowserStorageService(IJSRuntime jsRuntime)
{
    public ValueTask SetItemAsync(string key, string value) =>
        jsRuntime.InvokeVoidAsync("staffManagementStorage.set", key, value);

    public ValueTask<string?> GetItemAsync(string key) =>
        jsRuntime.InvokeAsync<string?>("staffManagementStorage.get", key);

    public ValueTask RemoveItemAsync(string key) =>
        jsRuntime.InvokeVoidAsync("staffManagementStorage.remove", key);

    public ValueTask DownloadFileAsync(string fileName, string contentType, byte[] bytes) =>
        jsRuntime.InvokeVoidAsync("staffManagementDownloads.saveFile", fileName, contentType, Convert.ToBase64String(bytes));
}
