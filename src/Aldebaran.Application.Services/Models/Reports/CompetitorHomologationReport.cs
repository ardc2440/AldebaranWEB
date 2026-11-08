using System.ComponentModel;

namespace Aldebaran.Application.Services.Models.Reports
{
    /// <summary>Filtros del reporte de Homologaciones con la competencia.</summary>
    public class CompetitorHomologationReportFilter
    {
        /// <summary>Ids de línea. Se usa solo si no hay artículos ni referencias.</summary>
        public IEnumerable<short> LineIds { get; set; } = Enumerable.Empty<short>();
        /// <summary>Ids de artículo. Se usa solo si no hay referencias.</summary>
        public IEnumerable<int> ItemIds { get; set; } = Enumerable.Empty<int>();
        /// <summary>Ids de referencia (incluye las homologaciones a nivel artículo de su artículo).</summary>
        public IEnumerable<int> ReferenceIds { get; set; } = Enumerable.Empty<int>();
        public IEnumerable<int> CompetitorCompanyIds { get; set; } = Enumerable.Empty<int>();
        /// <summary>Contiene (referencia del competidor).</summary>
        public string? CompetitorReference { get; set; }
        /// <summary>Contiene (nombre del producto del competidor).</summary>
        public string? CompetitorProductName { get; set; }
    }

    /// <summary>
    /// Caso de uso "Consultar reporte de homologaciones": árbol Compañía -> Línea -> Artículo -> Homologaciones,
    /// listo para pantalla, impresión y PDF.
    /// </summary>
    public sealed record CompetitorHomologationReportResult(IReadOnlyList<CompetitorHomologationReportCompany> Companies)
    {
        public static CompetitorHomologationReportResult Empty { get; } = new(Array.Empty<CompetitorHomologationReportCompany>());
        public bool HasData => Companies.Count > 0;
        public int HomologationCount => Companies.Sum(c => c.Lines.Sum(l => l.Items.Sum(i => i.Homologations.Count)));
    }

    public sealed record CompetitorHomologationReportCompany(int CompetitorCompanyId, string CompanyName, string Website,
        IReadOnlyList<CompetitorHomologationReportLine> Lines);

    public sealed record CompetitorHomologationReportLine(short LineId, string LineName, IReadOnlyList<CompetitorHomologationReportItem> Items);

    public sealed record CompetitorHomologationReportItem(int ItemId, string InternalReference, string ItemName,
        IReadOnlyList<CompetitorHomologationReportEntry> Homologations);

    /// <summary>Homologación: la referencia PROMOS es null cuando aplica a todo el artículo.</summary>
    public sealed record CompetitorHomologationReportEntry(
        int CompetitorHomologationId,
        int? ReferenceId,
        string? ReferenceCode,
        string? ReferenceName,
        string CompetitorReference,
        string CompetitorProductName,
        string? ProductUrl,
        decimal? CompetitorPrice,
        string? ImageFileName)
    {
        /// <summary>Referencia PROMOS legible: "01 - Amarillo" o "Todo el artículo".</summary>
        public string PromosReference => ReferenceId == null ? "Todo el artículo" : $"{ReferenceCode} - {ReferenceName}";
    }

    /// <summary>Caso de uso "Exportar reporte de homologaciones a Excel": una fila por homologación (sin imagen).</summary>
    public sealed class CompetitorHomologationReportExportRow
    {
        [DisplayName("Compañía")]
        public string CompanyName { get; init; } = string.Empty;
        [DisplayName("Sitio web")]
        public string Website { get; init; } = string.Empty;
        [DisplayName("Línea")]
        public string LineName { get; init; } = string.Empty;
        [DisplayName("Referencia interna")]
        public string InternalReference { get; init; } = string.Empty;
        [DisplayName("Artículo")]
        public string ItemName { get; init; } = string.Empty;
        [DisplayName("Referencia PROMOS")]
        public string PromosReference { get; init; } = string.Empty;
        [DisplayName("Referencia competidor")]
        public string CompetitorReference { get; init; } = string.Empty;
        [DisplayName("Producto competidor")]
        public string CompetitorProductName { get; init; } = string.Empty;
        [DisplayName("Precio competencia (COP)")]
        public decimal? CompetitorPrice { get; init; }
        [DisplayName("Enlace")]
        public string ProductUrl { get; init; } = string.Empty;
    }
}
