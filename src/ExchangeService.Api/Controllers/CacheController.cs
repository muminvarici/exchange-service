using ExchangeService.Core.Abstractions.Caching;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.Api.Controllers;

[Route("api/v1/[controller]")]
public class CacheController : ControllerBase
{
    private readonly ICacheProvider _cacheProvider;

    public CacheController(ICacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string key)
    {
        await _cacheProvider.DeleteAsync(key);
        return Ok();
    }
}