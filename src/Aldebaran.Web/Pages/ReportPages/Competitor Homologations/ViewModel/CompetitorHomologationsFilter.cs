using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Web.Pages.ReportPages.Item_References.Components;

namespace Aldebaran.Web.Pages.ReportPages.Competitor_Homologations.ViewModel
{
    /// <summary>
    /// Filtro aplicado en el reporte de Homologaciones con la competencia (resultado del diálogo de filtros):
    /// criterios del caso de uso y resumen legible (jerarquía y filtros de la competencia).
    /// </summary>
    public sealed record CompetitorHomologationsFilter(CompetitorHomologationReportFilter Criteria, ItemReferencesReportFilterSummary Summary);
}
