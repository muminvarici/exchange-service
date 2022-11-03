using ExchangeService.Application.Services.Abstractions;
using ExchangeService.Application.Services.Concretes;
using ExchangeService.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExchangeService.Application;

public static class Startup
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(configuration.GetSection(nameof(ApplicationSettings)));
        services.AddScoped<IExchangeUseCaseService, ExchangeUseCaseService>()
            ;
        return services;
    }
}