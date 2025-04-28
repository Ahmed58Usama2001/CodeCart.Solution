
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
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<StoreContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // Anything before is a service
        var app = builder.Build();
        // Anything after is a Middleware

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
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}
