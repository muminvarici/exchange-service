using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeService.Api.Requests;

public class GetExchangeRateRequest
{
    [FromQuery]
    public string SourceCurrencyCode { get; set; }

    [FromQuery]
    public string TargetCurrencyCode { get; set; }
}

public class GetExchangeRateRequestValidator : AbstractValidator<GetExchangeRateRequest>
{
    public GetExchangeRateRequestValidator()
    {
        RuleFor(w => w.SourceCurrencyCode).NotEmpty();
        RuleFor(w => w.TargetCurrencyCode).NotEmpty();
    }
}