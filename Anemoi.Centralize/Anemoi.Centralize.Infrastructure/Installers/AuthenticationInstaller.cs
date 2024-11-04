using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Application.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Anemoi.Centralize.Infrastructure.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>();
        
        var publicKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jwtSettings.PublicKeyPath);
        var publicSigningCredential = JwtSecurity.GetPublicSigningCredential(publicKeyPath);
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = publicSigningCredential,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero
        };
        var tokenParameterWithoutExpired = tokenValidationParameters.Clone();
        tokenParameterWithoutExpired.ValidateLifetime = false;
        services.AddSingleton(tokenParameterWithoutExpired);
        
        // Get token validation from everywhere
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = tokenValidationParameters;
        });
    }
}