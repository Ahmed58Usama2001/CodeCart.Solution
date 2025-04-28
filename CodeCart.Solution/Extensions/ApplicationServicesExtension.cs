using CodeCart.API.Helpers;
using CodeCart.Core.Repositories.Contract;
using CodeCart.Infrastructure.Repositories;

namespace CodeCart.API.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddAutoMapper(typeof(MappingProfiles));


        return services;
    }
}
