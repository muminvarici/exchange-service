using ExchangeService.Core.Entities.Enums;
using FluentValidation;

namespace ExchangeService.Api.Requests;

public class MakeExchangeRequest
{
    public string SourceCurrencyCode { get; set; }
    public string TargetCurrencyCode { get; set; }
    public decimal Amount { get; set; }
    public ExchangeDirection Direction { get; set; }
}

public class MakeExchangeRequestValidator : AbstractValidator<MakeExchangeRequest>
{
    public MakeExchangeRequestValidator()
    {
        RuleFor(w => w.Amount)
            .GreaterThan(0);
        RuleFor(w => w.Direction)
            .IsInEnum();
        RuleFor(w => w.SourceCurrencyCode)
            .NotEmpty();
        RuleFor(w => w.TargetCurrencyCode)
            .NotEmpty();
    }
}