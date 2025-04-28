
using CodeCart.API.Extensions;
using CodeCart.API.Middlewares;
using CodeCart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CodeCart.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddSwaggerServices();



        builder.Services.AddDbContext<StoreContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddApplicationServices();

        builder.Services.AddCors();

        // Anything before is a service
        var app = builder.Build();
        // Anything after is a Middleware

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseCors(x=>x.AllowAnyHeader().AllowAnyMethod()
        .WithOrigins("http://localhost:4200", "https://localhost:4200"));

        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;

        var _dbContext = services.GetRequiredService<StoreContext>();
        //Ask CLR for creating object from Context explicitly

        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        try
        {
            await _dbContext.Database.MigrateAsync(); //Updata-Database
            await StoreContextSeed.SeedAsync(_dbContext);
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogError(ex, "An Error has been occured during applying the migration");
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerMiddlewares();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles();

        app.Run();

    }
}
