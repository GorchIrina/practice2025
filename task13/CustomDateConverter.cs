using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace task13;

public class CustomDateConverter: JsonConverter<DateTime>
{
    private const string Form = "dd.MM.yyyy";
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString  = reader.GetString() ?? throw new JsonException("Дата не может быть null.");
        if(!DateTime.TryParseExact(dateString, Form, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            throw new JsonException("Некорректный формат даты");
        }
        return date;
    }
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Form));
    }
}
