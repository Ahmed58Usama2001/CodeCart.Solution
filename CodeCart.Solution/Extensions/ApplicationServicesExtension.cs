using CodeCart.API.Helpers;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using CodeCart.Infrastructure.Repositories;
using CodeCart.Service;

namespace CodeCart.API.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddSingleton<ICartService, CartService>();
        services.AddAutoMapper(typeof(MappingProfiles));


        return services;
    }
}
