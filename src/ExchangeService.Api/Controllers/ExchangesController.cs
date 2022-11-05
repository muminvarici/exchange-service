using ExchangeService.Api.Filters;
using ExchangeService.Api.Requests;
using ExchangeService.Application.Services.Abstractions;
using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using ExchangeService.Core.Services.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.Api.Controllers;

[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ExchangesController : ControllerBase
{
    private readonly IExchangeUseCaseService _useCaseService;
    private readonly IHolder _holder;

    public ExchangesController
    (
        IExchangeUseCaseService useCaseService,
        IHolder holder
    )
    {
        _useCaseService = useCaseService;
        _holder = holder;
    }

    
    /// <summary>
    /// Calculates the exchange amount for user money and creates an exchange record 
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SuccessResponse<decimal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MakeExchange([FromBody] MakeExchangeRequest request)
    {
        var result = await _useCaseService.MakeExchangeAsync(request.SourceCurrencyCode, request.TargetCurrencyCode, request.Amount, request.Direction);
        return Ok(new SuccessResponse<decimal>()
        {
            Data = result
        });
    }

    /// <summary>
    /// Calculates the remaining exchange count per user 
    /// </summary>
    [HttpGet("available-count")]
    [ProducesResponseType(typeof(SuccessResponse<int>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableCountForCustomer()
    {
        if (!_holder.CheckUser(false))
        {
            return Unauthorized();
        }

        var result = await _useCaseService.GetAvailableCountForCustomerAsync();
        return Ok(new SuccessResponse<int>()
        {
            Data = result
        });
    }

    /// <summary>
    /// Returns the valid exchange exchange rates 
    /// </summary>
    [HttpGet("rates")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<ExchangeRateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExchangeRates([FromQuery] GetExchangeRatesRequest request)
    {
        var result = await _useCaseService.GetValidRatesAsync(request.SourceCurrencyCode);
        return Ok(new SuccessResponse<IEnumerable<ExchangeRateDto>>()
        {
            Data = result
        });
    }

    /// <summary>
    /// Returns the valid exchange exchange rate for a specific currency
    /// </summary>
    [HttpGet("rate")]
    [ProducesResponseType(typeof(SuccessResponse<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExchangeRates([FromQuery] GetExchangeRateRequest request)
    {
        var rates = await _useCaseService.GetValidRatesAsync(request.SourceCurrencyCode);
        var result = rates.FirstOrDefault(w => w.Code.Equals(request.TargetCurrencyCode));

        return Ok(new SuccessResponse<ExchangeRateDto>()
        {
            Data = result
        });
    }
}