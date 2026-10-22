using Aldebaran.DataAccess.Core.Atributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Aldebaran.DataAccess.Core
{
    /// <summary>
    /// Ofusca en el Track los valores de las propiedades marcadas con <see cref="TrackMaskedAttribute"/>.
    /// Nunca se guarda el valor real ni un hash (evita recuperarlo por fuerza bruta); solo se indica
    /// si el valor fue modificado.
    /// </summary>
    public static class TrackValueMasker
    {
        public const string MaskedValue = "********";
        public const string ChangedSuffix = " (modificada)";

        private static readonly IReadOnlyDictionary<string, string?> NoValues = new Dictionary<string, string?>();

        public static TrackMaskedAttribute? GetMask(IProperty property)
            => property.PropertyInfo?.GetCustomAttribute<TrackMaskedAttribute>();

        /// <summary>
        /// Se llama ANTES de guardar: conserva en memoria (nunca se persiste) el valor original de las
        /// propiedades enmascaradas que cambiaron, para poder indicar despues que fueron modificadas.
        /// Si la entidad llego desconectada (DbSet.Update), EF no conoce el valor original; en ese caso,
        /// y solo para entidades con propiedades enmascaradas, se lee el valor actual en base de datos.
        /// </summary>
        public static async Task<IReadOnlyDictionary<string, string?>> CaptureOriginalValuesAsync(EntityEntry entry, CancellationToken ct = default)
        {
            if (entry.State != EntityState.Modified)
                return NoValues;

            var maskedProperties = entry.Properties
                .Where(p => p.IsModified && GetMask(p.Metadata) != null)
                .ToList();
            if (maskedProperties.Count == 0)
                return NoValues;

            var databaseValues = maskedProperties.Any(p => Equals(p.OriginalValue, p.CurrentValue))
                ? await entry.GetDatabaseValuesAsync(ct)
                : null;

            return maskedProperties.ToDictionary(
                p => p.Metadata.Name,
                p => (databaseValues != null ? databaseValues[p.Metadata] : p.OriginalValue)?.ToString());
        }

        public static string? Mask(TrackMaskedAttribute mask, string? value, string? originalValue, bool wasModified)
        {
            if (value == null)
                return null;

            return mask.JsonKeys.Length == 0
                ? MaskPlain(value, originalValue, wasModified)
                : MaskJson(value, originalValue, wasModified, mask.JsonKeys);
        }

        private static string MaskPlain(string value, string? originalValue, bool wasModified)
            => wasModified && value != originalValue ? MaskedValue + ChangedSuffix : MaskedValue;

        private static string MaskJson(string value, string? originalValue, bool wasModified, string[] keys)
        {
            var json = TryParse(value);
            if (json == null)
                return MaskPlain(value, originalValue, wasModified);

            var original = wasModified ? TryParse(originalValue) : null;
            var sensitiveProperties = json.Descendants().OfType<JProperty>()
                .Where(p => IsSensitive(p.Name, keys))
                .ToList();

            foreach (var property in sensitiveProperties)
                property.Value = HasChanged(property, original) ? MaskedValue + ChangedSuffix : MaskedValue;

            return json.ToString(Formatting.None);
        }

        private static bool HasChanged(JProperty property, JObject? original)
            => original != null && !JToken.DeepEquals(property.Value, original.SelectToken(property.Path));

        private static bool IsSensitive(string propertyName, string[] keys)
            => keys.Any(k => propertyName.Contains(k, StringComparison.OrdinalIgnoreCase));

        private static JObject? TryParse(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            try
            {
                return JToken.Parse(value) as JObject;
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }
    }
}
