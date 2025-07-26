using CodeCart.API.Extensions;
using CodeCart.API.Middlewares;
using CodeCart.API.SignalR;
using CodeCart.Core.Entities.Identity;
using CodeCart.Infrastructure.Data;
using CodeCart.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace CodeCart.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddSwaggerServices();

        builder.Services.AddDbContext<StoreContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddSingleton<IConnectionMultiplexer>((config) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connections string ");
            var configuration = ConfigurationOptions.Parse(connectionString, true);
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.AddApplicationServices();
        builder.Services.AddIdentityServices(builder.Configuration);
        builder.Services.AddHttpClient();
        builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("SignalRCorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials()
                      .SetIsOriginAllowed((host) => true);
            });
        });

        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<JwtBlacklistMiddleware>();

        app.UseCors("SignalRCorsPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<StoreContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(context, userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerMiddlewares();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapControllers();

        app.MapHub<NotificationHub>("/hub/notifications")
           .RequireCors("SignalRCorsPolicy");

        app.Run();
    }
}