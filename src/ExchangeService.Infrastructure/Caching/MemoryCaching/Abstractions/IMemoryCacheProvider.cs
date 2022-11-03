using ExchangeService.Core.Abstractions.Caching;

namespace ExchangeService.Infrastructure.Caching.MemoryCaching.Abstractions;

public interface IMemoryCacheProvider : ICacheProvider
{
    void Clear();
}