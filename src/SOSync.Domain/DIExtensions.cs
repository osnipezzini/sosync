using Microsoft.Extensions.DependencyInjection;
using SOSync.Common;
using SOSync.Domain.Interfaces;
using SOSync.Domain.Services;

namespace SOSync.Domain;

public static class DIExtensions
{

    public static void ConfigureDomainServices(this IServiceCollection services)
    {
        services.ConfigureCommonServices();
        services.AddScoped<IAPIService, APIService>();
    }
}
