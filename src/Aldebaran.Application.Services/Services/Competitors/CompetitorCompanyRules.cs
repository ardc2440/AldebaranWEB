using Aldebaran.Application.Services.Models.Competitors;

namespace Aldebaran.Application.Services.Competitors
{
    /// <summary>
    /// Reglas puras (sin BD) de la compañía de la competencia: normalización y validación del formulario.
    /// Longitudes = columnas de dbo.COMPETITOR_COMPANIES.
    /// </summary>
    public static class CompetitorCompanyRules
    {
        public const int CompanyNameMaxLength = 100;
        public const int WebsiteMaxLength = 250;
        public const int ContactPersonMaxLength = 50;
        public const int PhoneMaxLength = 20;

        /// <summary>Recorta espacios; los opcionales vacíos quedan en null.</summary>
        public static CompetitorCompanyInput Normalize(CompetitorCompanyInput input) => new()
        {
            CompanyName = (input?.CompanyName ?? string.Empty).Trim(),
            Website = (input?.Website ?? string.Empty).Trim(),
            ContactPerson = Optional(input?.ContactPerson),
            Phone = Optional(input?.Phone),
            CountryId = input?.CountryId,
            DepartmentId = input?.CountryId == null ? null : input.DepartmentId,
            CityId = input?.CountryId == null || input.DepartmentId == null ? null : input.CityId
        };

        /// <summary>Errores de captura (vacío = válido). Se espera la entrada ya normalizada.</summary>
        public static IReadOnlyList<string> Validate(CompetitorCompanyInput input)
        {
            var errors = new List<string>();
            Required(errors, input.CompanyName, "El nombre", CompanyNameMaxLength);
            Required(errors, input.Website, "El sitio web", WebsiteMaxLength);
            MaxLength(errors, input.ContactPerson, "El contacto", ContactPersonMaxLength);
            MaxLength(errors, input.Phone, "El teléfono", PhoneMaxLength);
            if (input.DepartmentId != null && input.CountryId == null)
                errors.Add("Para indicar el departamento/estado debe seleccionar el país.");
            if (input.CityId != null && input.DepartmentId == null)
                errors.Add("Para indicar la ciudad debe seleccionar el departamento/estado.");
            return errors;
        }

        private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static void Required(List<string> errors, string value, string field, int max)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{field} es obligatorio.");
            else
                MaxLength(errors, value, field, max);
        }

        private static void MaxLength(List<string> errors, string? value, string field, int max)
        {
            if (value != null && value.Length > max)
                errors.Add($"{field} admite máximo {max} caracteres.");
        }
    }
}
