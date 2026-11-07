namespace Aldebaran.Application.Services.Models.Competitors
{
    /// <summary>Caso de uso "Consultar compañías de la competencia": datos para el listado y el detalle.</summary>
    public sealed record CompetitorCompany(
        int CompetitorCompanyId,
        string CompanyName,
        string Website,
        string? ContactPerson,
        string? Phone,
        int? CountryId,
        string? CountryName,
        int? DepartmentId,
        string? DepartmentName,
        int? CityId,
        string? CityName,
        bool IsActive,
        DateTime CreationDate)
    {
        /// <summary>Ubicación legible: "Ciudad, Departamento, País" con los niveles informados.</summary>
        public string Location => string.Join(", ", new[] { CityName, DepartmentName, CountryName }.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    /// <summary>Caso de uso "Crear / editar compañía de la competencia": datos que captura el formulario.</summary>
    public sealed class CompetitorCompanyInput
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public int? CountryId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CityId { get; set; }
    }
}
