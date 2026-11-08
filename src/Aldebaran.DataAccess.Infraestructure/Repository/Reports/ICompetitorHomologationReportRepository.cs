using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICompetitorHomologationReportRepository
    {
        Task<IEnumerable<CompetitorHomologationReport>> GetReportDataAsync(CompetitorHomologationReportFilter filter, CancellationToken ct = default);

        /// <summary>
        /// Alcance homologado (homologaciones activas de compañías activas) para acotar el selector del reporte:
        /// artículos con homologación a nivel artículo y referencias con homologación a nivel referencia.
        /// </summary>
        Task<(IReadOnlyList<int> ItemIds, IReadOnlyList<int> ReferenceIds)> GetHomologatedScopeAsync(CancellationToken ct = default);
    }
}
