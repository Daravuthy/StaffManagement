using Microsoft.Playwright;
using Xunit;

namespace StaffManagement.EndToEndTests;

public sealed class StaffManagementUiTests : IClassFixture<BrowserAppFixture>
{
    private readonly BrowserAppFixture _fixture;

    public StaffManagementUiTests(BrowserAppFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UserCanCreateStaff()
    {
        var page = await _fixture.Browser.NewPageAsync();

        await page.GotoAsync($"{_fixture.WebUrl}/staffs");
        await page.WaitForURLAsync("**/staffs");
        await page.WaitForSelectorAsync("#staff-page-ready", new() { State = WaitForSelectorState.Attached });
        await page.WaitForSelectorAsync("#open-new-staff");

        var staffId = $"EMP{Random.Shared.Next(10000, 99999)}";
        await page.ClickAsync("#open-new-staff");
        await page.WaitForSelectorAsync("#staff-form-modal");
        await page.FillAsync("#staff-id", staffId);
        await page.FillAsync("#staff-full-name", "Playwright User");
        await page.FillAsync("#staff-birthday", "1992-11-05");
        await page.SelectOptionAsync("#staff-gender", new SelectOptionValue { Value = "Female" });
        await page.ClickAsync("#save-staff");
        await page.WaitForSelectorAsync("#staff-page-ready", new() { State = WaitForSelectorState.Attached });

        await page.FillAsync("#search-staff-id", staffId);
        await page.ClickAsync("#search-submit");
        await page.WaitForSelectorAsync($"tr[data-staff-id='{staffId}']");

        var resultRow = page.Locator($"tr[data-staff-id='{staffId}']");
        var resultsText = await resultRow.InnerTextAsync();
        Assert.Contains(staffId, resultsText);
        Assert.Contains("Playwright User", resultsText);
    }
}
