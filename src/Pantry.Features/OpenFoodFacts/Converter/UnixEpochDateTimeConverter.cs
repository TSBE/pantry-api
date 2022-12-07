using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Pantry.Features.OpenFoodFacts.Converters;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/standard/datetime/system-text-json-support#using-unix-epoch-date-format.
/// </summary>
public sealed partial class UnixEpochDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly DateTime _epoch = new(1970, 1, 1, 0, 0, 0);
    private static readonly Regex _regex = UnixEpochRegex();

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetInt64(out var x))
        {
            return _epoch.AddMilliseconds(x);
        }

        var formatted = reader.GetString()!;
        Match match = _regex.Match(formatted);

        if (!match.Success || !long.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var unixTime))
        {
            throw new JsonException();
        }

        return _epoch.AddMilliseconds(unixTime);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var unixTime = Convert.ToInt64((value - _epoch).TotalMilliseconds);

        var formatted = FormattableString.Invariant($"/Date({unixTime})/");
        writer.WriteStringValue(formatted);
    }

    [GeneratedRegex("^/Date\\(([+-]*\\d+)\\)/$", RegexOptions.CultureInvariant)]
    private static partial Regex UnixEpochRegex();
}
