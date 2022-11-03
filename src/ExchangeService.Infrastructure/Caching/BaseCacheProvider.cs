using ExchangeService.Core.Abstractions.Caching;

namespace ExchangeService.Infrastructure.Caching;

public abstract class BaseCacheProvider : ICacheProvider
{
    public abstract Task<T> GetAsync<T>(string key, CancellationToken cancellationToken);

    public abstract Task SetAsync<T>(string key, T value, CacheTime expiresIn, CancellationToken cancellationToken);

    public abstract Task<bool> KeyExistsAsync(string key);

    public abstract Task DeleteAsync(string key);
    public abstract ValueTask DisposeAsync();

    public TimeSpan GetTimeSpan(CacheTime cacheTime = CacheTime.None)
    {
        var timeSpan = TimeSpan.FromMinutes(15.0);
        return cacheTime switch
        {
            CacheTime.OneMinute => TimeSpan.FromMinutes(1.0),
            CacheTime.FiveMinute => TimeSpan.FromMinutes(5.0),
            CacheTime.FifteenMinutes => TimeSpan.FromMinutes(15.0),
            CacheTime.ThirtyMinutes => TimeSpan.FromMinutes(30.0),
            CacheTime.OneHour => TimeSpan.FromHours(1.0),
            CacheTime.ThreeHours => TimeSpan.FromHours(3.0),
            CacheTime.SixHours => TimeSpan.FromHours(6.0),
            CacheTime.TwelveHours => TimeSpan.FromHours(12.0),
            CacheTime.OneDay => TimeSpan.FromDays(1.0),
            CacheTime.ThreeDays => TimeSpan.FromDays(3.0),
            CacheTime.OneWeek => TimeSpan.FromDays(7.0),
            CacheTime.OneMonth => TimeSpan.FromDays(30.0),
            CacheTime.OneYear => TimeSpan.FromDays(365.0),
            _ => timeSpan
        };
    }

    public virtual void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}