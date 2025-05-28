using CodeCart.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CodeCart.Service.SecurityModule;
using CodeCart.Core.Services.Contracts.SecurityModule;
using CodeCart.Core.Entities.Identity;
using CodeCart.Core.Services.Contract.AccountModuleContracts;
using CodeCart.Service.AuthModuleService;

namespace CodeCart.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped(typeof(IGoogleAuthService), typeof(GoogleAuthService));
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequiredUniqueChars = 2;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        }).AddEntityFrameworkStores<StoreContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);
            var requiredKeyLength = 256 / 8;
            if (secretKey.Length < requiredKeyLength)
            {
                Array.Resize(ref secretKey, requiredKeyLength);
            }
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidateIssuer = true,
                ValidIssuer = configuration["JWT:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5) // Reduced clock skew to 5 minutes
            };
        });

        return services;
    }
}