using System.Text.Json;

namespace Aldebaran.Application.Services.TrackLogs
{
    /// <summary>Lee el JSON plano (campo → valor) que guarda log.track en ENTITY_KEY y DATA_LOG.</summary>
    internal static class TrackDataParser
    {
        public static IReadOnlyDictionary<string, string?> Parse(string? json)
        {
            var result = new Dictionary<string, string?>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(json))
                return result;

            try
            {
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                    return result;

                foreach (var property in document.RootElement.EnumerateObject())
                    result[property.Name] = ToText(property.Value);
            }
            catch (JsonException)
            {
                result["(dato)"] = json;
            }

            return result;
        }

        /// <summary>Llave legible: "Campo: valor, Campo: valor".</summary>
        public static string FormatKey(string? json)
        {
            var values = Parse(json);
            return values.Count == 0 ? json ?? string.Empty : string.Join(", ", values.Select(v => $"{v.Key}: {v.Value}"));
        }

        private static string? ToText(JsonElement value) => value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String => value.GetString(),
            _ => value.GetRawText()
        };
    }
}
