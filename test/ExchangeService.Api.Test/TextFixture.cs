using System.Collections.Generic;
using ExchangeService.Api.Requests;
using ExchangeService.Core.Entities.Enums;

namespace ExchangeService.Api.Test;

public class TextFixture
{
    public static List<object[]> GetListMembers =>
        new()
        {
            new object[] { GetMakeExchangeRequest(1, ExchangeDirection.Buy, "EUR") },
            new object[] { GetMakeExchangeRequest(13, ExchangeDirection.Buy, "EUR") },
            new object[] { GetMakeExchangeRequest(1, ExchangeDirection.Buy, "TRY") },
            new object[] { GetMakeExchangeRequest(16, ExchangeDirection.Buy, "TRY") },
            new object[] { GetMakeExchangeRequest(1, ExchangeDirection.Buy, "AED") },
            new object[] { GetMakeExchangeRequest(41, ExchangeDirection.Buy, "AED") },
            new object[] { GetMakeExchangeRequest(1, ExchangeDirection.Buy, "BAM") },
            new object[] { GetMakeExchangeRequest(63, ExchangeDirection.Buy, "BAM") },
        };

    public static MakeExchangeRequest GetMakeExchangeRequest(decimal amount, ExchangeDirection direction, string targetCurrency)
    {
        return new MakeExchangeRequest()
        {
            SourceCurrencyCode = "USD",
            Amount = amount,
            Direction = direction,
            TargetCurrencyCode = targetCurrency
        };
    }
}