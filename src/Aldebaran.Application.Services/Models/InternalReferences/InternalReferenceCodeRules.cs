using System.Text.RegularExpressions;

namespace Aldebaran.Application.Services.Models.InternalReferences
{
    /// <summary>
    /// Reglas de los consecutivos sugeridos CÓDIGO-NNN: deben caber en items.INTERNAL_REFERENCE (varchar(10)).
    /// CÓDIGO (máx. 5) + "-" + consecutivo (3 dígitos, 4 desde 1000, máx. 9999) = máx. 10 caracteres.
    /// </summary>
    public static class InternalReferenceCodeRules
    {
        public const int MaxCodeLength = 5;
        public const int MaxConsecutive = 9999;
        public const int MaxQuantity = 1000;

        private static readonly Regex AllowedCode = new("^[A-Z0-9Ñ]+( [A-Z0-9Ñ]+)*$", RegexOptions.Compiled);

        /// <summary>Código en mayúsculas y sin espacios al inicio o al final.</summary>
        public static string Normalize(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();

        /// <summary>Mensaje de error del código, o null si es válido.</summary>
        public static string? ValidateCode(string normalizedCode)
        {
            if (string.IsNullOrEmpty(normalizedCode))
                return "El código es obligatorio.";
            if (normalizedCode.Length > MaxCodeLength)
                return $"El código puede tener máximo {MaxCodeLength} caracteres.";
            if (!AllowedCode.IsMatch(normalizedCode))
                return "El código solo admite letras, números y un espacio entre palabras (sin guion ni otros signos).";
            return null;
        }

        /// <summary>Mensaje de error de la cantidad de disponibles requerida, o null si es válida.</summary>
        public static string? ValidateQuantity(int quantity) =>
            quantity < 1 || quantity > MaxQuantity ? $"La cantidad debe estar entre 1 y {MaxQuantity}." : null;

        /// <summary>Referencia sugerida: mínimo 3 dígitos con ceros a la izquierda (MU-005); desde 1000, 4 dígitos (VA-1227).</summary>
        public static string Format(string normalizedCode, int consecutive) => $"{normalizedCode}-{consecutive:000}";
    }
}
