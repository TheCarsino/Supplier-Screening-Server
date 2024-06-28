using System;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    private readonly string _dateTimeFormat;

    /* Function extracted from another source*/
    public CustomDateTimeConverter(string dateTimeFormat)
    {
        _dateTimeFormat = dateTimeFormat;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParseExact(reader.GetString(), _dateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }
        }

        throw new JsonException("Invalid date format.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_dateTimeFormat));
    }
}
