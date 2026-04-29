using Microsoft.Playwright;

namespace StaffManagement.EndToEndTests;

public sealed class BrowserAppFixture : IAsyncLifetime
{
    private readonly HttpClient _httpClient = new();

    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public string WebUrl => Environment.GetEnvironmentVariable("STAFF_MGMT_WEB_URL") ?? "http://localhost:5165";
    public string ApiHealthUrl => Environment.GetEnvironmentVariable("STAFF_MGMT_API_HEALTH_URL") ?? "http://localhost:5144/health";

    public async Task InitializeAsync()
    {
        await WaitForAsync(ApiHealthUrl);
        await WaitForAsync($"{WebUrl}/_framework/blazor.boot.json");

        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        Playwright?.Dispose();
        _httpClient.Dispose();
    }

    private async Task WaitForAsync(string url)
    {
        for (var attempt = 0; attempt < 60; attempt++)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
            }

            await Task.Delay(1000);
        }

        throw new TimeoutException($"Timed out waiting for {url}.");
    }
}
