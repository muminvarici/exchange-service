using ExchangeService.Infrastructure.Caching.RedisCaching.Abstractions;
using ExchangeService.Infrastructure.Caching.RedisCaching.Settings;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ExchangeService.Infrastructure.Caching.RedisCaching.Concretes;

public class ConnectionProvider : IConnectionProvider
{
    private RedisSettings _settings;
    private ConnectionMultiplexer _muxer;

    public ConnectionProvider
    (
        IOptions<RedisSettings> options
    )
    {
        _settings = options.Value;
    }

    public async ValueTask<IConnectionMultiplexer> GetConnection()
    {
        if (_muxer is not { IsConnected: true })
            await ConnectAsync();

        return _muxer;
    }

    private async Task ConnectAsync()
    {
        _muxer = await ConnectionMultiplexer.ConnectAsync(ConnectionString);
    }

    private string ConnectionString => $"{_settings.Endpoint}:{_settings.Port},password={_settings.Password}";

    public ValueTask DisposeAsync()
    {
        _settings = null;
        if (_muxer != null)
        {
            _muxer.Dispose();
            _muxer = null;
        }

        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        DisposeAsync().GetAwaiter().GetResult();
    }
}