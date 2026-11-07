namespace Aldebaran.Application.Services.Models.Competitors
{
    /// <summary>Caso de uso "Consultar homologaciones con la competencia": datos para el listado y el detalle.</summary>
    public sealed record CompetitorHomologation(
        int CompetitorHomologationId,
        int CompetitorCompanyId,
        string CompanyName,
        bool IsCompanyActive,
        string LineName,
        int ItemId,
        string InternalReference,
        string ItemName,
        int? ReferenceId,
        string? ReferenceCode,
        string? ReferenceName,
        string CompetitorReference,
        string CompetitorProductName,
        string? ProductUrl,
        decimal? CompetitorPrice,
        string? ImageFileName,
        bool IsActive,
        DateTime CreationDate)
    {
        /// <summary>Nivel de la homologación: "Artículo" o "Referencia".</summary>
        public string Level => ReferenceId == null ? "Artículo" : "Referencia";

        /// <summary>Artículo (y referencia) de PROMOS en forma legible: "[VA-1] Nombre - Referencia".</summary>
        public string PromosDescription => ReferenceName == null
            ? $"[{InternalReference}] {ItemName}"
            : $"[{InternalReference}] {ItemName} - {ReferenceName}";
    }

    /// <summary>Caso de uso "Crear / editar homologación": datos que captura el formulario (la imagen se gestiona aparte).</summary>
    public sealed class CompetitorHomologationInput
    {
        public int CompetitorCompanyId { get; set; }
        public int ItemId { get; set; }
        /// <summary>null = homologación a nivel de artículo.</summary>
        public int? ReferenceId { get; set; }
        public string CompetitorReference { get; set; } = string.Empty;
        public string CompetitorProductName { get; set; } = string.Empty;
        public string? ProductUrl { get; set; }
        /// <summary>Precio de la competencia en pesos colombianos.</summary>
        public decimal? CompetitorPrice { get; set; }
    }

    /// <summary>Filtros del listado de homologaciones.</summary>
    public sealed class CompetitorHomologationFilter
    {
        public int? CompetitorCompanyId { get; set; }
        public int? ItemId { get; set; }
        public bool IncludeInactive { get; set; }
        public string? SearchKey { get; set; }
    }
}
