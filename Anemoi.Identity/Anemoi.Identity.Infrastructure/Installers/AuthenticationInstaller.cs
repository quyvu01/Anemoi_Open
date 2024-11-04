using System;
using System.IO;
using Anemoi.BuildingBlock.Application.Abstractions;
using Anemoi.BuildingBlock.Application.Configurations;
using Anemoi.BuildingBlock.Application.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Anemoi.Identity.Infrastructure.Installers;

public sealed class AuthenticationInstaller : IInstaller
{
    public void InstallerServices(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSetting = configuration.GetSection(nameof(JwtSetting)).Get<JwtSetting>()!;
        services.AddSingleton(jwtSetting);

        var privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jwtSetting.PrivateKeyPath);
        var privateSecurityKey = JwtSecurity.GetPrivateSecurityKey(privateKeyPath);
        var privateSigningCredential = JwtSecurity.GetPrivateSigningCredential(privateSecurityKey);
        services.AddSingleton(privateSigningCredential);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = privateSecurityKey,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuer = false,
            ClockSkew = TimeSpan.Zero
        };
        var tokenValidationParametersForSystem = tokenValidationParameters.Clone();
        tokenValidationParametersForSystem.ValidateLifetime = false;

        // Get token validation from everywhere
        services.AddSingleton(tokenValidationParametersForSystem);
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