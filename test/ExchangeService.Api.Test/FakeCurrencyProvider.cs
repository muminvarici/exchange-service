using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeService.Core.Extensions;
using ExchangeService.Core.Services;
using ExchangeService.Core.Services.Responses;
using ExchangeService.Infrastructure.CurrencyProviders.FixerIo;

namespace ExchangeService.Api.Test;

public class FakeCurrencyProvider : ICurrencyProvider
{
    public async Task<IEnumerable<ExchangeRateDto>> GetExchangeRatesAsync(string currencyCode)
    {
        await Task.CompletedTask;
        const string json = "{\"Success\":true,\"TimeStamp\":\"1667497384000000\",\"Base\":\"USD\",\"Date\":\"2022-11-03\",\"Rates\":{\"AED\":3.673016,\"AFN\":88.49606,\"ALL\":119.304736,\"AMD\":395.291513,\"ANG\":1.799044,\"AOA\":488.044203,\"ARS\":157.939617,\"AUD\":1.58853,\"AWG\":1.8,\"AZN\":1.701824,\"BAM\":2.004571,\"BBD\":2.015507,\"BDT\":101.867508,\"BGN\":2.00521,\"BHD\":0.376952,\"BIF\":2054,\"BMD\":1,\"BND\":1.42054,\"BOB\":6.897616,\"BRL\":5.104802,\"BSD\":0.998171,\"BTC\":0.000049423421,\"BTN\":82.758267,\"BWP\":13.525882,\"BYN\":2.520588,\"BYR\":19600,\"BZD\":2.012125,\"CAD\":1.373305,\"CDF\":2044.00031,\"CHF\":1.01275,\"CLF\":0.034275,\"CLP\":945.700301,\"CNY\":7.301601,\"COP\":5091.92,\"CRC\":616.366963,\"CUC\":1,\"CUP\":26.5,\"CVE\":112.104229,\"CZK\":25.0802,\"DJF\":177.720106,\"DKK\":7.63259,\"DOP\":54.074964,\"DZD\":140.463341,\"EGP\":24.250396,\"ERN\":15,\"ETB\":52.900812,\"EUR\":1.025435,\"FJD\":2.314498,\"FKP\":0.882525,\"GBP\":0.894185,\"GEL\":2.74498,\"GGP\":0.882525,\"GHS\":13.697482,\"GIP\":0.882525,\"GMD\":58.618042,\"GNF\":8704.999972,\"GTQ\":7.806003,\"GYD\":208.807146,\"HKD\":7.849915,\"HNL\":24.830149,\"HRK\":7.728403,\"HTG\":129.771391,\"HUF\":416.689972,\"IDR\":15781.1,\"ILS\":3.579345,\"IMP\":0.882525,\"INR\":82.73905,\"IQD\":1460,\"IRR\":42299.999902,\"ISK\":148.589847,\"JEP\":0.882525,\"JMD\":153.225682,\"JOD\":0.709023,\"JPY\":148.256028,\"KES\":121.497251,\"KGS\":83.730105,\"KHR\":4148.000031,\"KMF\":499.224963,\"KPW\":899.999862,\"KRW\":1425.559871,\"KWD\":0.31069,\"KYD\":0.831826,\"KZT\":465.821799,\"LAK\":16945.00024,\"LBP\":1526.106889,\"LKR\":366.842885,\"LRD\":153.703963,\"LSL\":18.224984,\"LTL\":2.952739,\"LVL\":0.60489,\"LYD\":5.009708,\"MAD\":10.957503,\"MDL\":19.265439,\"MGA\":4275.000226,\"MKD\":63.279894,\"MMK\":2096.244382,\"MNT\":3359.775161,\"MOP\":8.070945,\"MRO\":356.999828,\"MUR\":44.600168,\"MVR\":15.404983,\"MWK\":1025.499715,\"MXN\":19.670196,\"MYR\":4.737981,\"MZN\":63.830184,\"NAD\":18.225003,\"NGN\":439.169896,\"NIO\":35.999497,\"NOK\":10.56934,\"NPR\":132.413638,\"NZD\":1.730415,\"OMR\":0.384498,\"PAB\":0.998171,\"PEN\":3.9675,\"PGK\":3.513032,\"PHP\":58.8755,\"PKR\":221.519784,\"PLN\":4.81521,\"PYG\":7251.110758,\"QAR\":3.640977,\"RON\":5.025298,\"RSD\":120.234978,\"RUB\":62.250041,\"RWF\":1050,\"SAR\":3.759859,\"SBD\":8.230592,\"SCR\":14.583547,\"SDG\":568.502967,\"SEK\":11.185897,\"SGD\":1.42183,\"SHP\":1.377403,\"SLL\":17469.999709,\"SOS\":569.500741,\"SRD\":29.631047,\"STD\":20697.981008,\"SVC\":8.734376,\"SYP\":2512.530034,\"SZL\":18.225003,\"THB\":38.009837,\"TJS\":10.031926,\"TMT\":3.51,\"TND\":3.236498,\"TOP\":2.416498,\"TRY\":18.617699,\"TTD\":6.772678,\"TWD\":32.237005,\"TZS\":2327.83103,\"UAH\":36.865484,\"UGX\":3787.185413,\"USD\":1,\"UYU\":40.472079,\"UZS\":11127.491978,\"VND\":24865,\"VUV\":125.149891,\"WST\":2.844937,\"XAF\":672.304074,\"XAG\":0.051424,\"XAU\":0.000614,\"XCD\":2.70255,\"XDR\":0.777813,\"XOF\":660.500677,\"XPF\":121.501824,\"YER\":250.24979,\"ZAR\":18.399804,\"ZMK\":9001.194287,\"ZMW\":16.190984,\"ZWL\":321.999592}}";
        var data = json.DeSerialize<GetLatestResponse>();
        return data!.Rates.Select(w => new ExchangeRateDto
        {
            Code = w.Key,
            Value = w.Value,
            RateDate = new DateTime(data.Date.Year, data.Date.Month, data.Date.Day).Add(data.TimeStamp.TimeOfDay),
        });
    }
}