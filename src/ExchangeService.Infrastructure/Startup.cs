using ExchangeService.Application;
using ExchangeService.Core.Abstractions.Caching;
using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using ExchangeService.Core.Infrastructure.Holders.Concretes;
using ExchangeService.Core.Repositories;
using ExchangeService.Core.Services;
using ExchangeService.Infrastructure.Caching.MemoryCaching;
using ExchangeService.Infrastructure.Caching.RedisCaching.Abstractions;
using ExchangeService.Infrastructure.Caching.RedisCaching.Concretes;
using ExchangeService.Infrastructure.Caching.RedisCaching.Settings;
using ExchangeService.Infrastructure.CurrencyProviders.FixerIo;
using ExchangeService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace ExchangeService.Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Data");
        var useRedis = configuration.GetValue<bool>("ApplicationSettings:UseRedis");

        services.AddDbContext<ApplicationDbContext>(connectionString, optionsAction => { });

        if (useRedis) services.AddRedis(configuration);
        else services.AddMemoryCache(configuration);

        return services.AddHttpContextAccessor()
            .AddApplicationServices(configuration)
            .AddScoped<IHolder>(serviceProvider =>
            {
                var service = serviceProvider.GetRequiredService<Holder>();
                service.InitializeData();
                return service;
            })
            .AddScoped<Holder>()
            .AddScoped<ICurrencyProvider, FixerIoCurrencyProvider>()
            .AddScoped(typeof(IRepository<>), typeof(GenericRepository<>))
            .AddServiceClients(configuration);
    }

    private static void AddDbContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContextPool<DbContext, TContext>((serviceProvider, option) =>
        {
            option.UseNpgsql(connectionString);
            if (optionsAction == null)
                return;
            optionsAction(option);
        });
    }

    private static IServiceCollection AddServiceClients(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(nameof(FixerIoSettings));
        services.Configure<FixerIoSettings>(section);
        services.Configure<RedisSettings>(section);

        var setting = new FixerIoSettings();
        section.Bind(setting);

        services
            .AddRefitClient<IFixerIoApi>()
            .ConfigureHttpClient(c => c.BaseAddress = setting.Url);
        return services;
    }

    private static IServiceCollection AddMemoryCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
        services.AddMemoryCache();
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(RedisSettings));

        if (string.IsNullOrWhiteSpace(config.GetValue<string>(nameof(RedisSettings.Endpoint))))
            throw new ArgumentNullException(nameof(RedisSettings));
//AddStackExchangeRedisCache memory cache idistributed olarak çalışıyor mu

        services.AddSingleton<ICacheProvider>(serviceProvider => serviceProvider.GetRequiredService<IRedisCacheProvider>());
        services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();
        services.AddSingleton<IConnectionProvider, ConnectionProvider>();

        services.Configure<RedisSettings>(config);
        return services;
    }
}