using StackExchange.Redis;

namespace ExchangeService.Infrastructure.Caching.RedisCaching.Abstractions;

public interface IConnectionProvider : IAsyncDisposable, IDisposable
{
    ValueTask<IConnectionMultiplexer> GetConnection();
}