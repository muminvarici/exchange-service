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
        var useRedis = configuration.GetValue<bool>("ApplicationSettings:UseRedis");

        if (useRedis) services.AddRedis(configuration);
        else services.AddMemoryCache(configuration);

        var usePostgres = configuration.GetValue<bool>("ApplicationSettings:UsePostgres");
        if (usePostgres)
        {
            var connectionString = configuration.GetConnectionString("Data");
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
        }
        else
        {
            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("ExchangeDb"));
        }

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

    private static void AddMemoryCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheProvider, MemoryCacheProvider>();
        services.AddMemoryCache();
    }

    private static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(RedisSettings));

        if (string.IsNullOrWhiteSpace(config.GetValue<string>(nameof(RedisSettings.Endpoint))))
            throw new ArgumentNullException(nameof(RedisSettings));
//AddStackExchangeRedisCache memory cache idistributed olarak çalışıyor mu

        services.AddSingleton<ICacheProvider>(serviceProvider => serviceProvider.GetRequiredService<IRedisCacheProvider>());
        services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();
        services.AddSingleton<IConnectionProvider, ConnectionProvider>();

        services.Configure<RedisSettings>(config);
    }
}