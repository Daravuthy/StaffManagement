using System.Net;
using System.Net.Http.Json;
using StaffManagement.Shared.Enums;
using StaffManagement.Shared.Requests;
using StaffManagement.Shared.Responses;

namespace StaffManagement.IntegrationTests;

public sealed class StaffApiEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public StaffApiEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task StaffCrudAndExports_WorkEndToEnd()
    {
        using var client = _factory.CreateClient();

        var createRequest = new StaffUpsertRequest
        {
            StaffId = $"EMP{Random.Shared.Next(10000, 99999)}",
            FullName = "Integration User",
            Birthday = new DateOnly(1995, 4, 12),
            Gender = Gender.Female
        };

        var createResponse = await client.PostAsJsonAsync("/api/staffs", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<StaffDto>();
        Assert.NotNull(created);

        var searchResponse = await client.GetAsync($"/api/staffs?staffId={createRequest.StaffId}");
        searchResponse.EnsureSuccessStatusCode();
        var searchResults = await searchResponse.Content.ReadFromJsonAsync<List<StaffDto>>();
        Assert.Single(searchResults!);

        var excelResponse = await client.GetAsync($"/api/reports/staffs/excel?staffId={createRequest.StaffId}");
        excelResponse.EnsureSuccessStatusCode();
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelResponse.Content.Headers.ContentType?.MediaType);

        var pdfResponse = await client.GetAsync($"/api/reports/staffs/pdf?staffId={createRequest.StaffId}");
        pdfResponse.EnsureSuccessStatusCode();
        Assert.Equal("application/pdf", pdfResponse.Content.Headers.ContentType?.MediaType);
    }
}
