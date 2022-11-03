using Refit;

namespace ExchangeService.Infrastructure.CurrencyProviders.FixerIo;

public class GetLatestRequest
{
    [Query]
    [AliasAs("base")]
    public string BaseCurrency { get; }

    public GetLatestRequest(string baseCurrency)
    {
        BaseCurrency = baseCurrency;
    }
}