using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.Web.Pages.ReportPages.Item_References.Components;

namespace Aldebaran.Web.Pages.ReportPages.Provider_References.ViewModel
{
    /// <summary>
    /// Filtro aplicado en el reporte de Referencias del Proveedor (resultado del diálogo de filtros):
    /// criterios del caso de uso, proveedor elegido (para mostrarlo) y resumen legible de jerarquía y condiciones.
    /// </summary>
    public sealed record ProviderReferencesFilter(ProviderReferenceReportFilter Criteria, Provider? Provider, ItemReferencesReportFilterSummary Summary);
}
