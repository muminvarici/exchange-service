using Refit;

namespace ExchangeService.Infrastructure.CurrencyProviders.FixerIo;

public interface IFixerIoApi
{
    [Get("/latest")]
    Task<IApiResponse<GetLatestResponse>> GetLatestAsync(GetLatestRequest request, [Header("apikey")] string apiKey);
}