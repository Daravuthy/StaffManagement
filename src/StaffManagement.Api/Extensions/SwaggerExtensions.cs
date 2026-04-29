using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace StaffManagement.Api.Extensions;

public static class SwaggerExtensions
{
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration config)
    {
        if (!config.GetValue<bool>("SwaggerSettings:Enable"))
        {
            return app;
        }

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Staff Management API v1");
        });

        return app;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration config)
    {
        if (!config.GetValue<bool>("SwaggerSettings:Enable"))
        {
            return services;
        }

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Staff Management Web API",
                Version = "v1",
                Description = "Staff management assignment API built with ASP.NET Core 9.0.",
                Contact = new OpenApiContact
                {
                    Name = "Staff Management",
                    Email = "admin@staff.local"
                },
                License = new OpenApiLicense
                {
                    Name = "Internal Assignment License",
                    Url = new Uri("https://github.com/")
                }
            });

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

                var xmlFile = $"{assembly.GetName().Name}.xml";
                var xmlPath = Path.Combine(baseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            }

        });

        return services;
    }
}
