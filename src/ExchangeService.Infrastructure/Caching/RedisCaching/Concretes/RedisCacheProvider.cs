using ExchangeService.Core.Abstractions.Caching;
using ExchangeService.Core.Extensions;
using ExchangeService.Infrastructure.Caching.RedisCaching.Abstractions;
using ExchangeService.Infrastructure.Caching.RedisCaching.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ExchangeService.Infrastructure.Caching.RedisCaching.Concretes;

public class RedisCacheProvider : BaseCacheProvider, IRedisCacheProvider
{
    private readonly IConnectionProvider _connectionProvider;
    private RedisSettings _settings;

    public RedisCacheProvider(
        IOptions<RedisSettings> options,
        IConnectionProvider connectionProvider
    )
    {
        _connectionProvider = connectionProvider;
        _settings = options.Value;
    }

    private async ValueTask<IDatabase> GetDatabase() => (await _connectionProvider.GetConnection()).GetDatabase(_settings.Db);


    public override async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        var cache = await GetDatabase();
        var value = await cache.StringGetAsync(key);

        return value.IsNullOrEmpty
            ? default
            : value.ToString().DeSerialize<T>();
    }

    public override async Task<bool> SetAsync<T>(string key, T value, CacheTime expiryTime, CancellationToken cancellationToken)
    {
        var cache = await GetDatabase();
        var result = await cache.StringSetAsync(key, value.Serialize(), GetTimeSpan(expiryTime));
        return result;
    }

    public override async Task DeleteAsync(string key)
    {
        var cache = await GetDatabase();
        cache.KeyDelete(key);
    }

    public override async Task<bool> KeyExistsAsync(string key)
    {
        var cache = await GetDatabase();
        return await cache.KeyExistsAsync(key);
    }

    public async Task<bool> LockAsync(string key, CacheTime expiryTime, string value)
    {
        var cache = await GetDatabase();
        return await cache.LockTakeAsync(key, value, GetTimeSpan(expiryTime));
    }

    public async Task<bool> UnlockAsync(string key, string value)
    {
        var cache = await GetDatabase();
        return await cache.LockReleaseAsync(key, value);
    }

    public override async ValueTask DisposeAsync()
    {
        await _connectionProvider.DisposeAsync();
        _settings = null;
        GC.SuppressFinalize(this);
    }
}