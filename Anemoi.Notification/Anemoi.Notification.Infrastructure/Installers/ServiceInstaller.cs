using Amazon.Runtime;
using Amazon.S3;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Infrastructure.GeneralInstaller;
using Anemoi.BuildingBlock.Infrastructure.Services;
using Anemoi.Notification.Application.Abstractions;
using Anemoi.Notification.Domain;
using Anemoi.Notification.Infrastructure.DataContext;
using Anemoi.Notification.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anemoi.Notification.Infrastructure.Installers;

public sealed class ServiceInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEfRepositoriesAsScope<NotificationDbContext>(typeof(INotificationDomainAssemblyMarker).Assembly);
        services.AddEfUnitOfWorkAsScope<NotificationDbContext>();
        services.TryAddTransient<IEmailService, EmailService>();
        services.AddScoped<IUserIdSetter, UserService>();
        services.AddScoped<IFileService, S3FileService>();
        services.AddTransient<IAmazonS3>(sp =>
        {
            var s3Setting = sp.GetRequiredService<S3Setting>();
            var config = new AmazonS3Config { ServiceURL = s3Setting.ServiceUrl };
            var credentials = new BasicAWSCredentials(s3Setting.AccessKey, s3Setting.SecretKey);
            var client = new AmazonS3Client(credentials, config);
            return client;
        });
        services.AddScoped<IUserIdGetter>(sp => sp.GetRequiredService<IUserIdSetter>() as UserService);
        services.AddScoped<IWorkspaceIdSetter, WorkspaceService>();
        services.AddScoped<IWorkspaceIdGetter>(sp => sp.GetRequiredService<IWorkspaceIdSetter>() as WorkspaceService);
        services.AddScoped<IAdministratorSetter, AdministratorService>();
        services.AddScoped<IAdministratorGetter>(sp => sp.GetRequiredService<IAdministratorSetter>() as AdministratorService);
        services.AddScoped<IApplicationPolicySetter, ApplicationPolicyService>();
        services.AddScoped<IApplicationPolicyGetter>(sp => sp.GetRequiredService<IApplicationPolicySetter>() as ApplicationPolicyService);
        services.AddScoped<ITokenSetter, TokenService>();
        services.AddScoped<ITokenGetter>(sp => sp.GetRequiredService<ITokenSetter>() as TokenService);
        services.AddScoped<ISmtpProvider, SmtpProvider>();
    }
}