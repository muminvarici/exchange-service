using ExchangeService.Core.Services;
using ExchangeService.Core.Services.Responses;
using Microsoft.Extensions.Options;

namespace ExchangeService.Infrastructure.CurrencyProviders.FixerIo
{
    public class FixerIoCurrencyProvider : ICurrencyProvider
    {
        private readonly IFixerIoApi _fixerIoApi;
        private readonly FixerIoSettings _fixerIoSettings;

        public FixerIoCurrencyProvider
        (
            IFixerIoApi fixerIoApi,
            IOptions<FixerIoSettings> fixerIoSettings
        )
        {
            _fixerIoSettings = fixerIoSettings.Value;
            _fixerIoApi = fixerIoApi;
        }

        public async Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(string currencyCode)
        {
            var response = await _fixerIoApi.GetLatestAsync(new GetLatestRequest(currencyCode), _fixerIoSettings.ApiKey);
            if (!response.IsSuccessStatusCode || response.Content?.Success != true) return Enumerable.Empty<ExchangeRateDto>();

            return response.Content.Rates.Select(w => new ExchangeRateDto
            {
                Code = w.Key,
                Value = w.Value,
                RateDate = new DateTime(response.Content.Date.Year, response.Content.Date.Month, response.Content.Date.Day).Add(response.Content.TimeStamp.TimeOfDay),
            });
        }
    }
}