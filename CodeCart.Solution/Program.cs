
using CodeCart.API.Extensions;
using CodeCart.API.Middlewares;
using CodeCart.Infrastructure.Data;
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
        builder.Services.AddDbContext<StoreContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        builder.Services.AddSingleton<IConnectionMultiplexer>((config) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connections string ");
            var configuration = ConfigurationOptions.Parse(connectionString,true);

            return ConnectionMultiplexer.Connect(configuration);
        });
        builder.Services.AddApplicationServices();
        builder.Services.AddCors();

        var app = builder.Build();

        // Exception handling middleware (now works with either approach)
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseCors(x => x.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:4200", "https://localhost:4200"));

        // Apply migrations
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<StoreContext>();
                await context.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(context);
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
        app.UseAuthorization();
        app.MapControllers();
        app.Run();

    }
}
