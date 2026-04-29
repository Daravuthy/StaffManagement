using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using StaffManagement.Web;
using StaffManagement.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = ResolveApiBaseUrl(builder.HostEnvironment.BaseAddress, builder.Configuration);

builder.Services.AddScoped<BrowserStorageService>();
builder.Services.AddScoped<StaffApiClient>();
builder.Services.AddHttpClient("Api", client => client.BaseAddress = new Uri(apiBaseUrl));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

await builder.Build().RunAsync();

static string ResolveApiBaseUrl(string webBaseAddress, IConfiguration configuration)
{
    var webUri = new Uri(webBaseAddress);

    if (webUri.Scheme == Uri.UriSchemeHttps)
    {
        return webUri.Port switch
        {
            44330 => "https://localhost:44325/",
            7290 => "https://localhost:7171/",
            _ => configuration["ApiBaseUrlHttps"]
                 ?? configuration["ApiBaseUrl"]
                 ?? throw new InvalidOperationException("API base URL configuration is missing.")
        };
    }

    return webUri.Port switch
    {
        49885 => "http://localhost:64701/",
        5165 => "http://localhost:5144/",
        _ => configuration["ApiBaseUrl"]
             ?? throw new InvalidOperationException("API base URL configuration is missing.")
    };
}
