using Microsoft.EntityFrameworkCore;
using StaffManagement.Api.Data;
using StaffManagement.Api.Extensions;
using StaffManagement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IStaffReportService, StaffReportService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

var app = builder.Build();

app.UseSwaggerDocumentation(builder.Configuration);
app.UseCors("Frontend");
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

await DbSeeder.SeedAsync(app.Services);

app.Run();

public partial class Program;
