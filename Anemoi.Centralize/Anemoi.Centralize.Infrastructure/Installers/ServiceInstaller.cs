using Amazon.Runtime;
using Amazon.S3;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anemoi.Centralize.Application.Abstractions;
using Anemoi.Centralize.Application.ContractAssemblies;
using Anemoi.Centralize.Application.Filters;
using Anemoi.Centralize.Infrastructure.Services;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddTransient<IRequestClientService, RequestClientService>();
        services.AddScoped<AutoMapDataFilter>();
        services.AddScoped<IFileService, S3FileService>();
        services.AddScoped<IImageProcessor, ImageProcessor>();
        services.AddTransient<IAmazonS3>(sp =>
        {
            var s3Setting = sp.GetRequiredService<S3Setting>();
            var config = new AmazonS3Config { ServiceURL = s3Setting.ServiceUrl };
            var credentials = new BasicAWSCredentials(s3Setting.AccessKey, s3Setting.SecretKey);
            var client = new AmazonS3Client(credentials, config);
            return client;
        });
        services.AddScoped<IDataMappableService>(sp =>
            new DataMappableService(sp, ContractAssembly.GetAllContractAssemblies()));
        services.AddScoped<ICustomUserIdSetter, CustomUserIdService>();
        services.AddScoped<ICustomUserIdGetter>(sp => sp.GetRequiredService<ICustomUserIdSetter>() as CustomUserIdService);
        services.AddScoped<ICustomWorkspaceIdSetter, CustomWorkspaceIdService>();
        services.AddScoped<ICustomWorkspaceIdGetter>(sp => sp.GetRequiredService<ICustomWorkspaceIdSetter>() as CustomWorkspaceIdService);
    }
}