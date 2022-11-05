using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.Api.Requests;

public class GetExchangeRatesRequest
{
    [FromQuery]
    public string SourceCurrencyCode { get; set; }
}

public class GetExchangeRatesRequestValidator : AbstractValidator<GetExchangeRatesRequest>
{
    public GetExchangeRatesRequestValidator()
    {
        RuleFor(w => w.SourceCurrencyCode).NotEmpty();
    }
}