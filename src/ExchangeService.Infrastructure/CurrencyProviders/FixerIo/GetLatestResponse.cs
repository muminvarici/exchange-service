using System.Text.Json.Serialization;
using ExchangeService.Core.Infrastructure.Converters;

namespace ExchangeService.Infrastructure.CurrencyProviders.FixerIo;

public class GetLatestResponse
{
    public bool Success { get; set; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime TimeStamp { get; set; }

    public string Base { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly Date { get; set; }

    public Dictionary<string, decimal> Rates { get; set; }
}