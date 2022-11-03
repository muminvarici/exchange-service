using ExchangeService.Core.Entities.Enums;

namespace ExchangeService.Core.Entities;

public class ExchangeLog : EntityBase
{
    public string SourceCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public decimal SourceAmount { get; set; }
    public decimal TargetAmount { get; set; }
    public ExchangeDirection Direction { get; set; }
    public DateTime RateDate { get; set; } 
}