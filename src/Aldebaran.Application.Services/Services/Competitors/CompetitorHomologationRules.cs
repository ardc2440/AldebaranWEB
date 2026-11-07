using Aldebaran.Application.Services.Models.Competitors;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Reglas puras (sin BD) de la homologación: normalización y validación del formulario.
    /// Longitudes = columnas de dbo.COMPETITOR_HOMOLOGATIONS.
    /// </summary>
    public static class CompetitorHomologationRules
    {
        public const int CompetitorReferenceMaxLength = 50;
        public const int CompetitorProductNameMaxLength = 150;
        public const int ProductUrlMaxLength = 500;
        public const decimal MaxPrice = 9_999_999_999_999_999.99m; // decimal(18,2)

        /// <summary>Recorta espacios; enlace vacío = null y, si no trae esquema, se antepone https://.</summary>
        public static CompetitorHomologationInput Normalize(CompetitorHomologationInput input) => new()
        {
            CompetitorCompanyId = input?.CompetitorCompanyId ?? 0,
            ItemId = input?.ItemId ?? 0,
            ReferenceId = input?.ReferenceId is > 0 ? input.ReferenceId : null,
            CompetitorReference = (input?.CompetitorReference ?? string.Empty).Trim(),
            CompetitorProductName = (input?.CompetitorProductName ?? string.Empty).Trim(),
            ProductUrl = NormalizeUrl(input?.ProductUrl),
            CompetitorPrice = input?.CompetitorPrice
        };

        /// <summary>Errores de captura (vacío = válido). Se espera la entrada ya normalizada.</summary>
        public static IReadOnlyList<string> Validate(CompetitorHomologationInput input)
        {
            var errors = new List<string>();
            if (input.CompetitorCompanyId <= 0)
                errors.Add("Debe seleccionar la compañía de la competencia.");
            if (input.ItemId <= 0)
                errors.Add("Debe seleccionar el artículo.");
            Required(errors, input.CompetitorReference, "La referencia del competidor", CompetitorReferenceMaxLength);
            Required(errors, input.CompetitorProductName, "El nombre del producto del competidor", CompetitorProductNameMaxLength);
            if (input.ProductUrl != null)
            {
                if (input.ProductUrl.Length > ProductUrlMaxLength)
                    errors.Add($"El enlace admite máximo {ProductUrlMaxLength} caracteres.");
                else if (!IsWebUrl(input.ProductUrl))
                    errors.Add("El enlace no es una dirección web válida.");
            }
            if (input.CompetitorPrice is < 0)
                errors.Add("El precio de la competencia no puede ser negativo.");
            else if (input.CompetitorPrice > MaxPrice)
                errors.Add("El precio de la competencia supera el valor máximo permitido.");
            return errors;
        }

        public static bool IsWebUrl(string value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            && uri.Host.Contains('.');

        private static string? NormalizeUrl(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            var url = value.Trim();
            return url.Contains("://", StringComparison.Ordinal) ? url : "https://" + url;
        }

        private static void Required(List<string> errors, string value, string field, int max)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{field} es obligatoria.");
            else if (value.Length > max)
                errors.Add($"{field} admite máximo {max} caracteres.");
        }
    }
}
