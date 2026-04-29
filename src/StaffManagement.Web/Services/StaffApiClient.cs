using System.Net;
using System.Net.Http.Json;
using StaffManagement.Shared.Requests;
using StaffManagement.Shared.Responses;

namespace StaffManagement.Web.Services;

public sealed class StaffApiClient(HttpClient httpClient, BrowserStorageService storageService)
{
    public async Task<List<StaffDto>> SearchAsync(StaffSearchRequest request)
    {
        var response = await httpClient.GetAsync(BuildUrl("api/staffs", request));
        await EnsureSuccessAsync(response);
        return await response.Content.ReadFromJsonAsync<List<StaffDto>>();
    }

    public async Task CreateAsync(StaffUpsertRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/staffs", request);
        await EnsureSuccessAsync(response);
    }

    public async Task UpdateAsync(Guid id, StaffUpsertRequest request)
    {
        var response = await httpClient.PutAsJsonAsync($"api/staffs/{id}", request);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteAsync(Guid id)
    {
        var response = await httpClient.DeleteAsync($"api/staffs/{id}");
        await EnsureSuccessAsync(response);
    }

    public async Task ExportAsync(StaffSearchRequest request, string format, string fileName)
    {
        var response = await httpClient.GetAsync(BuildUrl($"api/reports/staffs/{format}", request));
        await EnsureSuccessAsync(response);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType
                          ?? "application/octet-stream";
        await storageService.DownloadFileAsync(fileName, contentType, bytes);
    }

    private static string BuildUrl(string path, StaffSearchRequest request)
    {
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.StaffId))
        {
            query.Add($"staffId={Uri.EscapeDataString(request.StaffId)}");
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            query.Add($"fullName={Uri.EscapeDataString(request.FullName)}");
        }

        if (request.Gender.HasValue)
        {
            query.Add($"gender={(int)request.Gender.Value}");
        }

        if (request.BirthdayFrom.HasValue)
        {
            query.Add($"birthdayFrom={request.BirthdayFrom.Value:yyyy-MM-dd}");
        }

        if (request.BirthdayTo.HasValue)
        {
            query.Add($"birthdayTo={request.BirthdayTo.Value:yyyy-MM-dd}");
        }

        return query.Count == 0 ? path : $"{path}?{string.Join("&", query)}";
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var message = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
            ? "The request failed."
            : message);
    }
}
