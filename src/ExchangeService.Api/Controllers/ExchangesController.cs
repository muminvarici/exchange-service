using ExchangeService.Api.Requests;
using ExchangeService.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.Api.Controllers;

[Route("api/v1/[controller]")]
public class ExchangesController : ControllerBase
{
    private readonly IExchangeUseCaseService _useCaseService;

    public ExchangesController(IExchangeUseCaseService useCaseService)
    {
        _useCaseService = useCaseService;
    }

    [HttpPost]
    public async Task<IActionResult> MakeExchange([FromBody] MakeExchangeRequest request)
    {
        var result = await _useCaseService.MakeExchangeAsync(request.SourceCurrencyCode, request.TargetCurrencyCode, request.Amount, request.Direction);
        return Ok(result);
    }

    [HttpGet("available-count")]
    public async Task<IActionResult> GetAvailableCountForCustomer()
    {
        var result = await _useCaseService.GetAvailableCountForCustomerAsync();
        return Ok(result);
    }
}