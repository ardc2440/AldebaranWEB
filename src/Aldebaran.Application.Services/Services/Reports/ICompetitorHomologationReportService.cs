using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>Casos de uso del reporte de Homologaciones con la competencia.</summary>
    public interface ICompetitorHomologationReportService
    {
        Task<CompetitorHomologationReportResult> GetReportAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default);
        Task<IReadOnlyList<CompetitorHomologationReportExportRow>> GetExportAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default);

        /// <summary>
        /// Del catálogo de referencias, solo las homologadas (activas): todas las referencias de un artículo con homologación
        /// a nivel artículo y las referencias con homologación propia. Para acotar el selector jerárquico del reporte.
        /// </summary>
        Task<IReadOnlyList<ItemReference>> FilterHomologatedAsync(IEnumerable<ItemReference> catalog, CancellationToken ct = default);
    }
}
