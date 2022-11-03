using ExchangeService.Application.Services.Abstractions;
using ExchangeService.Application.Settings;
using ExchangeService.Core.Abstractions.Caching;
using ExchangeService.Core.Entities;
using ExchangeService.Core.Entities.Enums;
using ExchangeService.Core.Extensions;
using ExchangeService.Core.Infrastructure.Exceptions;
using ExchangeService.Core.Infrastructure.Holders.Abstractions;
using ExchangeService.Core.Repositories;
using ExchangeService.Core.Services;
using ExchangeService.Core.Services.Responses;
using Microsoft.Extensions.Options;

namespace ExchangeService.Application.Services.Concretes;

public class ExchangeUseCaseService : IExchangeUseCaseService
{
    private readonly ICurrencyProvider _currencyProvider;
    private readonly ICacheProvider _cacheProvider;
    private readonly IHolder _holder;
    private readonly IRepository<ExchangeLog> _exchangeLogRepository;
    private readonly ApplicationSettings _applicationSettings;

    public ExchangeUseCaseService
    (
        ICurrencyProvider currencyProvider,
        ICacheProvider cacheProvider,
        IHolder holder,
        IRepository<ExchangeLog> exchangeLogRepository,
        IOptions<ApplicationSettings> applicationSettings
    )
    {
        _applicationSettings = applicationSettings.Value;
        _currencyProvider = currencyProvider;
        _cacheProvider = cacheProvider;
        _holder = holder;
        _exchangeLogRepository = exchangeLogRepository;
    }

    public async Task<decimal> MakeExchangeAsync(string sourceCurrency, string targetCurrency, decimal amount, ExchangeDirection direction)
    {
        var (value, currency) = await CalculateAmount(targetCurrency, targetCurrency, amount);
        var entity = new ExchangeLog()
        {
            Direction = direction,
            SourceAmount = amount,
            TargetCurrencyCode = targetCurrency,
            TargetAmount = value,
            SourceCurrencyCode = sourceCurrency,
            RateDate = currency.RateDate
        };
        await CheckUserExchangeCountPerHourAsync();
        await _exchangeLogRepository.InsertAsync(entity);
        await _exchangeLogRepository.SaveAllAsync();
        return value;
    }

    const string _ratePerUserCacheKey = "exchange:rateperuser:{0}";

    public async Task<int> GetAvailableCountForCustomerAsync()
    {
        var currentCount = await GetCurrentCountForCustomerAsync();
        var availableCount = _applicationSettings.MaxExchangeCount - currentCount;
        return Math.Max(availableCount, 0);
    }

    private async Task<int> GetCurrentCountForCustomerAsync()
    {
        var cacheKey = _ratePerUserCacheKey.Format(_holder.UserId.ToString());
        var currentCount = await _cacheProvider.GetAsync<int>(cacheKey);
        return currentCount;
    }

    private async Task CheckUserExchangeCountPerHourAsync()
    {
        var currentCount = await GetCurrentCountForCustomerAsync();
        currentCount++;
        var cacheKey = _ratePerUserCacheKey.Format(_holder.UserId.ToString());
        await _cacheProvider.SetAsync(cacheKey, currentCount, CacheTime.OneHour);

        if (currentCount >= _applicationSettings.MaxExchangeCount) throw new ServiceException(ServiceExceptionType.Forbidden, "MaxExchangeCountExceeded", $"User can't exchange more than {_applicationSettings.MaxExchangeCount} per hour");
    }

    public Task<IEnumerable<ExchangeRateDto>> GetValidRatesAsync(string currency)
    {
        return _cacheProvider.GetAsync<IEnumerable<ExchangeRateDto>>($"exchange:rates:{currency}", CacheTime.OneHour, async () =>
        {
            var data = await _currencyProvider.GetExchangeRatesAsync(currency).ToListAsync();
            return data;
        });
    }

    public async Task<(decimal value, ExchangeRateDto currency )> CalculateAmount(string sourceCurrency, string targetCurrency, decimal amount)
    {
        var rates = await GetValidRatesAsync(sourceCurrency).ToListAsync();
        if (!rates.Any()) throw new ServiceException(ServiceExceptionType.NotFound, "currency.not.found", $"Currency {sourceCurrency} not found");
        var currency = rates.FirstOrDefault(w => w.Code.Equals(targetCurrency, StringComparison.InvariantCultureIgnoreCase));
        if (currency == null) throw new ServiceException(ServiceExceptionType.NotFound, "currency.not.found", $"Currency {targetCurrency} not found for source {sourceCurrency}");
        return (Math.Round(currency.Value * amount, 2), currency);
    }
}