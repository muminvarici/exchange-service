namespace ExchangeService.Core.Extensions;

public static class StringExtensions
{
    public static string Format(this string str, params object[] args)
    {
        return string.Format(str, args);
    }
}