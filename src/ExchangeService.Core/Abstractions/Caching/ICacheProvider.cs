namespace ExchangeService.Core.Abstractions.Caching;

public interface ICacheProvider : IAsyncDisposable, IDisposable
{
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken cancellationToken = default);
    Task<bool> KeyExistsAsync(string key);
    Task DeleteAsync(string key);
    TimeSpan GetTimeSpan(CacheTime cacheTime = CacheTime.None);
}