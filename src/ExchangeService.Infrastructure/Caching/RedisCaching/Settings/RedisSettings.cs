namespace ExchangeService.Infrastructure.Caching.RedisCaching.Settings;

public class RedisSettings
{
    public string Endpoint { get; set; }
    public int Port { get; set; }
    public int Db { get; set; }
    public int TimeOut { get; set; }
    public string Password { get; set; }
    public string User { get; set; }
}