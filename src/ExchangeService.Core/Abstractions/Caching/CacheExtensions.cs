using System.Collections.Concurrent;

namespace ExchangeService.Core.Abstractions.Caching;

public static class CacheExtensions
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> KeyedSemaphoreDictionary = new();
    public static T Get<T>(
        this ICacheProvider cacheProvider,
        string key,
        CacheTime cacheTime,
        Func<T> acquire)
    {
        return cacheProvider.GetAsync<T>(key, cacheTime, async () => await Task.FromResult<T>(acquire())).GetAwaiter().GetResult();
    }

    public static async Task<T> GetAsync<T>(
        this ICacheProvider cacheProvider,
        string key,
        CacheTime cacheTime,
        Func<Task<T>> acquire,
        CancellationToken token = default)
    {
        var value = await cacheProvider.GetAsync<T>(key, token);
        if (value != null)
            return value;
        var keyedSemaphoreSlim = KeyedSemaphoreDictionary.GetOrAdd(key, (Func<string, SemaphoreSlim>)(s => new SemaphoreSlim(1, 1)));
        await keyedSemaphoreSlim.WaitAsync(token);
        try
        {
            value = await cacheProvider.GetAsync<T>(key, token);
            if (value != null)
                return value;
            value = await acquire();
            if (cacheTime > 0)
                await cacheProvider.SetAsync<T>(key, value, cacheTime, token);
        }
        finally
        {
            keyedSemaphoreSlim.Release();
            KeyedSemaphoreDictionary.TryRemove(key, out var _);
        }

        return value;
    }

    public static T Get<T>(this ICacheProvider cacheManager, string key, Func<T> acquire) => cacheManager.Get<T>(key, CacheTime.None, acquire);

    public static async Task<T> GetAsync<T>(
        this ICacheProvider cacheManager,
        string key,
        Func<Task<T>> acquire,
        CancellationToken token = default)
    {
        return await cacheManager.GetAsync<T>(key, CacheTime.None, acquire, token);
    }
}