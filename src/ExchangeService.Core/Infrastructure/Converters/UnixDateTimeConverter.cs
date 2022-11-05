using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeService.Core.Infrastructure.Converters;

public class UnixDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return DateTime.UnixEpoch.AddSeconds(reader.GetInt64());
        _ = long.TryParse(reader.GetString(), out var val);
        return DateTime.UnixEpoch.AddSeconds(val / 1000000);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue((value - DateTime.UnixEpoch).TotalMilliseconds + "000");
    }
}