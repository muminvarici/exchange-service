namespace ExchangeService.Core.Services.Responses;

public class ExchangeRateDto
{
    public string Code { get; set; }
    public decimal Value { get; set; }
    public DateTime RateDate { get; set; }
}