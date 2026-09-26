using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>Casos de uso del reporte de Artículos y Referencias.</summary>
    public interface IItemReferencesReportService
    {
        /// <summary>Consulta el reporte organizado en árbol Línea -> Artículo -> Referencias (pantalla, impresión, PDF).</summary>
        Task<ItemReferencesReportResult> GetReportAsync(ItemReferencesReportFilter filter, CancellationToken ct = default);

        /// <summary>Consulta el reporte en filas planas para exportar a Excel.</summary>
        Task<IReadOnlyList<ItemReferencesReportExportRow>> GetExportAsync(ItemReferencesReportFilter filter, CancellationToken ct = default);
    }
}
