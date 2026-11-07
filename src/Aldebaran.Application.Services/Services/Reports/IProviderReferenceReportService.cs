using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    /// <summary>Casos de uso del reporte de Referencias del Proveedor.</summary>
    public interface IProviderReferenceReportService
    {
        /// <summary>Consulta el reporte en árbol Proveedor -> Línea -> Artículo -> Referencias (-> Bodegas) para pantalla, impresión y PDF.</summary>
        Task<ProviderReferenceReportResult> GetReportAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default);

        /// <summary>Consulta el reporte en filas planas (una por proveedor-referencia, con o sin inventario) para exportar a Excel.</summary>
        Task<ProviderReferenceReportExport> GetExportAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default);
    }
}
