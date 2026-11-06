using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IProviderReferenceReportRepository
    {
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(ProviderReferenceReportFilter filter, CancellationToken ct = default);

        [Obsolete("Filtro armado como texto. Se retira en T3 (la pantalla pasa a usar el filtro tipado).")]
        Task<IEnumerable<ProviderReferenceReport>> GetProviderReferenceReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
