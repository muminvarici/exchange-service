using System.Collections.Concurrent;
using ExchangeService.Core.Abstractions.Caching;
using ExchangeService.Infrastructure.Caching.MemoryCaching.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace ExchangeService.Infrastructure.Caching.MemoryCaching;

public class MemoryCacheProvider : BaseCacheProvider, IMemoryCacheProvider
{
    private readonly ConcurrentDictionary<string, object> _allKeys = new();
    private readonly IMemoryCache _memoryCache;
    private CancellationTokenSource _cancellationTokenSource;

    public MemoryCacheProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(
        TimeSpan cacheTime)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions().AddExpirationToken(new CancellationChangeToken(_cancellationTokenSource.Token)).RegisterPostEvictionCallback(PostEviction);
        cacheEntryOptions.AbsoluteExpirationRelativeToNow = cacheTime;
        return cacheEntryOptions;
    }

    private string AddKey(string key)
    {
        _allKeys.TryAdd(key, null);
        return key;
    }

    public override Task DeleteAsync(string key)
    {
        TryRemoveKey(key);
        return Task.CompletedTask;
    }

    public override ValueTask DisposeAsync()
    {
        Clear();
        _memoryCache.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    private void TryRemoveKey(string key) => _allKeys.TryRemove(key, out var _);

    private void PostEviction(object key, object value, EvictionReason reason, object state)
    {
        if (reason == EvictionReason.Replaced)
            return;
        TryRemoveKey(key.ToString());
    }

    public override Task<T> GetAsync<T>(string key, CancellationToken cancellationToken) => Task.FromResult(_memoryCache.Get<T>(key));

    private void Set<T>(string key, T value, CacheTime expiresIn = CacheTime.None) => _memoryCache.Set(AddKey(key), value, GetMemoryCacheEntryOptions(GetTimeSpan(expiresIn)));

    public override async Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken cancellationToken)
    {
        Set(key, value, expiresIn);
        await Task.CompletedTask;
    }

    private bool IsSet(string key) => _memoryCache.TryGetValue(key, out var _);

    public override Task<bool> KeyExistsAsync(string key) => Task.FromResult(IsSet(key));

    public void Clear()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        _allKeys.Clear();
    }
}