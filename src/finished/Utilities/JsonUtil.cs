namespace MaterializedViewProcessor.Utilities;

public static class JsonUtil
{
    public static readonly JsonSerializerOptions DefaultSerializerSettings = new()
    {
        PropertyNamingPolicy = null,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = null,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(null),
            new TimespanJsonConverter()
        }
    };

    public static readonly JsonSerializerOptions CamelCaseSerializerSettings = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            new TimespanJsonConverter()
        }
    };

    public static string SerializeObjectWithoutReferenceLoop(object? value)
    {
        return JsonSerializer.Serialize(value, DefaultSerializerSettings);
    }
}
public class TimespanJsonConverter : JsonConverter<TimeSpan>
{
    public static readonly string TimeSpanFormatString = @"d\.hh\:mm\:ss\:FFF";

    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? stringRead = reader.GetString();
        if (string.IsNullOrWhiteSpace(stringRead))
        {
            return TimeSpan.Zero;
        }

        if (!TimeSpan.TryParseExact(stringRead, TimeSpanFormatString, null, out var parsedTimeSpan))
        {
            throw new FormatException($"Input timespan is not in an expected format : expected {Regex.Unescape(TimeSpanFormatString)}. Please retrieve this key as a string and parse manually.");
        }

        return parsedTimeSpan;
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        string timespanFormatted = $"{value.ToString(TimeSpanFormatString)}";
        writer.WriteStringValue(timespanFormatted);
    }
}

