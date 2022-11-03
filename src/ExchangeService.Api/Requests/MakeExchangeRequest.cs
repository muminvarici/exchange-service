using ExchangeService.Core.Entities.Enums;

namespace ExchangeService.Api.Requests;

public class MakeExchangeRequest
{
    public string SourceCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public decimal Amount { get; set; }
    public ExchangeDirection Direction { get; set; }
}