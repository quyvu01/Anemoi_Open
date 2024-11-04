using System.Reflection;
using Anemoi.BuildingBlock.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class SwaggerInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        // Gen Swagger
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Anemoi.Centralize API", Version = "v1" });
            // Add filters to fix enums
            options.AddEnumsWithValuesFixFilters();
            // Add this line to add example
            options.ExampleFilters();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. 
                    Enter 'Bearer' [space] and then your token in the text input below.
                    Example: 'Bearer some_jwt",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
            });

            options.AddSecurityDefinition("XApiKey", new OpenApiSecurityScheme
            {
                Description = "XApiKey is required for some API",
                Type = SecuritySchemeType.ApiKey,
                Name = "XApiKey",
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "XApiKey"
                        },
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
            });

            // Add the following lines to add swagger more
            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        // Explicit opt-in - needs to be placed after AddSwaggerGen()
        services.AddSwaggerGenNewtonsoftSupport();
        // Add this line to add example
        // Get package from nuget: Swashbuckle.AspNetCore.Filter
        services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
    }
}