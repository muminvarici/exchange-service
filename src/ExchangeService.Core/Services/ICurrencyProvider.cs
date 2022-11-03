using ExchangeService.Core.Services.Responses;

namespace ExchangeService.Core.Services
{
    public interface ICurrencyProvider
    {
        Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(string currencyCode);
    }
}