namespace ExchangeService.Core.Abstractions.Caching;

public enum CacheTime : byte
{
    None,
    OneMinute,
    FiveMinute,
    FifteenMinutes,
    ThirtyMinutes,
    OneHour,
    ThreeHours,
    SixHours,
    TwelveHours,
    OneDay,
    ThreeDays,
    OneWeek,
    OneMonth,
    OneYear,
}