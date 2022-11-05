using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeService.Api.Requests;
using ExchangeService.Application.Services.Abstractions;
using ExchangeService.Core.Entities;
using ExchangeService.Core.Extensions;
using ExchangeService.Core.Infrastructure.Exceptions;
using ExchangeService.Core.Repositories;
using ExchangeService.Core.Services;
using ExchangeService.Core.Services.Responses;
using ExchangeService.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ExchangeService.Api.Test;

public class ApiTests
{
    private readonly IConfiguration _configuration;
    private readonly ServiceCollection _services;
    private readonly IExchangeUseCaseService _useCaseService;
    private readonly ICurrencyProvider _currencyProvider;
    private readonly IRepository<ExchangeLog> _repository;

    public ApiTests()
    {
        _services = new ServiceCollection();
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.Test.json");
        _configuration = configurationBuilder.Build();
        _services.AddInfrastructureServices(_configuration);
        var serviceProvider = _services.BuildServiceProvider();
        _useCaseService = serviceProvider.GetService<IExchangeUseCaseService>();
        _currencyProvider = serviceProvider.GetService<ICurrencyProvider>();
        _repository = serviceProvider.GetService<IRepository<ExchangeLog>>();
    }

    [Fact]
    public void ConfigureServices_RegistersDependenciesCorrectly()
    {
        _useCaseService.Should().NotBeNull();
        _useCaseService.Should().NotBeNull();
        _currencyProvider.Should().NotBeNull();
        _repository.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(TextFixture.GetListMembers), MemberType = typeof(TextFixture))]
    public async Task ValidRates_ShouldNotBeNull(MakeExchangeRequest request)
    {
        await GetRates(request);
    }

    private async Task<List<ExchangeRateDto>> GetRates(MakeExchangeRequest request)
    {
        var rates = await _useCaseService.GetValidRatesAsync(request.SourceCurrencyCode).ToListAsync();
        rates.Should().NotBeNullOrEmpty();
        rates.Should().Contain(w => w.Code.Equals(request.TargetCurrencyCode, StringComparison.InvariantCultureIgnoreCase));
        return rates;
    }

    [Theory]
    [MemberData(nameof(TextFixture.GetListMembers), MemberType = typeof(TextFixture))]
    public async Task CalculateAmount_ShouldBeValid(MakeExchangeRequest request)
    {
        var rates = await GetRates(request);
        var target = rates.FirstOrDefault(w => w.Code.Equals(request.TargetCurrencyCode, StringComparison.InvariantCultureIgnoreCase));
        target.Should().NotBeNull();
        target?.Value.Should().BeGreaterThan(0);
        var (value, currency) = await _useCaseService.CalculateAmount(request.SourceCurrencyCode, request.TargetCurrencyCode, request.Amount);
        currency.Code.Should().Be(currency.Code);
        value.Should().BeGreaterThan(0);

        (value * 100).Should().Be((int)(value * 100)); // check presicion count
    }

    [Theory]
    [MemberData(nameof(TextFixture.GetListMembers), MemberType = typeof(TextFixture))]
    public async Task MakeExchange_ShouldBeValid(MakeExchangeRequest request)
    {
        var (calculatedValue, currency) = await _useCaseService.CalculateAmount(request.SourceCurrencyCode, request.TargetCurrencyCode, request.Amount);
        var value = await _useCaseService.MakeExchangeAsync(request.SourceCurrencyCode, request.TargetCurrencyCode, request.Amount, request.Direction);

        calculatedValue.Should().Be(value);

        var last = _repository.Table.LastOrDefault();
        last.Should().NotBeNull();
        last.Direction.Should().Be(request.Direction);
        last.RateDate.Should().Be(currency.RateDate);
        last.SourceAmount.Should().Be(request.Amount);
        last.TargetAmount.Should().Be(value);
        last.SourceCurrencyCode.Should().Be(request.SourceCurrencyCode);
        last.TargetCurrencyCode.Should().Be(request.TargetCurrencyCode);
        last.Id.Should().BePositive();
    }
}