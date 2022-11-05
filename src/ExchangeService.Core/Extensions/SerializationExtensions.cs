using System.Text.Json;
using ExchangeService.Core.Infrastructure.Converters;

namespace ExchangeService.Core.Extensions;

public static class SerializationExtensions
{
    private static readonly JsonSerializerOptions? _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    static SerializationExtensions()
    {
        _options.Converters.Add(new DateOnlyJsonConverter());
        _options.Converters.Add(new UnixDateTimeConverter());
    }

    public static string Serialize(this object data) => JsonSerializer.Serialize(data, _options);
    public static T? DeSerialize<T>(this string data) => JsonSerializer.Deserialize<T>(data, _options);
}