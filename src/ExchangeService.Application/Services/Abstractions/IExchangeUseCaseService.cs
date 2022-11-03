using ExchangeService.Core.Entities.Enums;
using ExchangeService.Core.Services.Responses;

namespace ExchangeService.Application.Services.Abstractions;

public interface IExchangeUseCaseService
{
    Task<decimal> MakeExchangeAsync(string sourceCurrency, string targetCurrency, decimal amount, ExchangeDirection direction);
    Task<IEnumerable<ExchangeRateDto>> GetValidRatesAsync(string currency);
    Task<(decimal value, ExchangeRateDto currency)> CalculateAmount(string sourceCurrency, string targetCurrency, decimal amount);
    Task<int> GetAvailableCountForCustomerAsync();
}