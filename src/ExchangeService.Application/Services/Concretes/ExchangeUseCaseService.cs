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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExchangeService.Application.Services.Concretes;

public class ExchangeUseCaseService : IExchangeUseCaseService
{
    private readonly ICurrencyProvider _currencyProvider;
    private readonly ICacheProvider _cacheProvider;
    private readonly IHolder _holder;
    private readonly IRepository<ExchangeLog> _exchangeLogRepository;
    private readonly ILogger<ExchangeUseCaseService> _logger;
    private readonly ApplicationSettings _applicationSettings;

    public ExchangeUseCaseService
    (
        ICurrencyProvider currencyProvider,
        ICacheProvider cacheProvider,
        IHolder holder,
        IRepository<ExchangeLog> exchangeLogRepository,
        IOptions<ApplicationSettings> applicationSettings,
        ILogger<ExchangeUseCaseService> logger
    )
    {
        _applicationSettings = applicationSettings.Value;
        _currencyProvider = currencyProvider;
        _cacheProvider = cacheProvider;
        _holder = holder;
        _exchangeLogRepository = exchangeLogRepository;
        _logger = logger;
    }

    public async Task<decimal> MakeExchangeAsync(string sourceCurrency, string targetCurrency, decimal amount, ExchangeDirection direction)
    {
        await CheckUserExchangeCountPerHourAsync();
        var (value, currency) = await CalculateAmount(sourceCurrency, targetCurrency, amount);
        var entity = new ExchangeLog()
        {
            Direction = direction,
            SourceAmount = amount,
            TargetCurrencyCode = targetCurrency,
            TargetAmount = value,
            SourceCurrencyCode = sourceCurrency,
            RateDate = currency.RateDate
        };
        await _exchangeLogRepository.InsertAsync(entity);
        await _exchangeLogRepository.SaveAllAsync();

        _logger.LogInformation("Exchange completed for user {entity}", entity);

        return value;
    }

    const string _ratePerUserCacheKey = "exchange:rateperuser:{0}";

    public async Task<int> GetAvailableCountForCustomerAsync()
    {
        var currentCount = await TryGetCurrentCountForCustomerAsync();
        var availableCount = _applicationSettings.MaxExchangeCount - currentCount;
        _logger.LogInformation("Available count:{availableCount} for user {user} ", availableCount, _holder.UserId);

        return Math.Max(availableCount, 0);
    }

    private async Task<int> TryGetCurrentCountForCustomerAsync()
    {
        var cacheKey =string.Format( _ratePerUserCacheKey,_holder.UserId.ToString());
        var currentCount = await _cacheProvider.GetAsync<int>(cacheKey);
        if (currentCount == 0)
        {
            //currentCount = await _exchangeLogRepository.Table.CountAsync(w => w.CreatedAt > DateTime.Now.AddHours(-1) && w.CreatedBy == _holder.UserId);
        }

        _logger.LogInformation("Current count:{currentCount} for user {user} ", currentCount, _holder.UserId);

        return currentCount;
    }

    private async Task CheckUserExchangeCountPerHourAsync()
    {
        var currentCount = await TryGetCurrentCountForCustomerAsync();
        if (currentCount >= _applicationSettings.MaxExchangeCount) throw new ServiceException(ServiceExceptionType.Forbidden, "MaxExchangeCountExceeded", $"User can't exchange more than {_applicationSettings.MaxExchangeCount} per hour");
        currentCount++;
        _logger.LogInformation("Current count increased to {currentCount} for user {user} ", currentCount, _holder.UserId);

        var cacheKey =string.Format( _ratePerUserCacheKey,_holder.UserId.ToString());

        await _cacheProvider.SetAsync(cacheKey, currentCount, CacheTime.OneHour);
    }

    public Task<IEnumerable<ExchangeRateDto>> GetValidRatesAsync(string currency)
    {
        return _cacheProvider.GetAsync<IEnumerable<ExchangeRateDto>>($"exchange:rates:{currency}", CacheTime.ThirtyMinutes, async () =>
        {
            var data = await _currencyProvider.GetExchangeRatesAsync(currency).ToListAsync();
            _logger.LogInformation("Valid rates fetched");
            return data;
        });
    }

    public async Task<(decimal value, ExchangeRateDto currency )> CalculateAmount(string sourceCurrency, string targetCurrency, decimal amount)
    {
        var rates = await GetValidRatesAsync(sourceCurrency).ToListAsync();
        if (!rates.Any()) throw new ServiceException(ServiceExceptionType.NotFound, "currency.not.found", $"Currency {sourceCurrency} not found");
        var currency = rates.FirstOrDefault(w => w.Code.Equals(targetCurrency, StringComparison.InvariantCultureIgnoreCase));
        if (currency == null) throw new ServiceException(ServiceExceptionType.NotFound, "currency.not.found", $"Currency {targetCurrency} not found for source {sourceCurrency}");
        var value = Math.Round(currency.Value * amount, 2);
        _logger.LogInformation("Calculated exchange rate {data}", new
        {
            sourceCurrency,
            targetCurrency,
            amount,
            calculatedAmount = value,
            currency.RateDate
        });
        return (value, currency);
    }
}