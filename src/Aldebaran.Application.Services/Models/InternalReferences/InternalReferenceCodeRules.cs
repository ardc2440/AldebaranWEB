namespace Aldebaran.Application.Services.Models.InternalReferences
{
    /// <summary>Reglas del código de referencia interna: la referencia CÓDIGO-NNN debe caber en items.INTERNAL_REFERENCE (varchar(10)).</summary>
    public static class InternalReferenceCodeRules
    {
        public const int MaxCodeLength = 6;

        /// <summary>Código en mayúsculas y sin espacios al inicio o al final.</summary>
        public static string Normalize(string? code) => (code ?? string.Empty).Trim().ToUpperInvariant();

        /// <summary>Mensaje de error del código, o null si es válido.</summary>
        public static string? ValidateCode(string normalizedCode)
        {
            if (string.IsNullOrEmpty(normalizedCode))
                return "El código es obligatorio.";
            if (normalizedCode.Any(char.IsWhiteSpace))
                return "El código no puede contener espacios.";
            if (normalizedCode.Length > MaxCodeLength)
                return $"El código puede tener máximo {MaxCodeLength} caracteres.";
            return null;
        }

        /// <summary>Mensaje de error de la cantidad a generar, o null si es válida.</summary>
        public static string? ValidateQuantity(int quantity) =>
            quantity < 1 ? "La cantidad a generar debe ser mayor que cero." : null;
    }
}
