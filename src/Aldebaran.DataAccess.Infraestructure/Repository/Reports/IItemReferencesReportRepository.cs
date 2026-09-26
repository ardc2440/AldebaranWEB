using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IItemReferencesReportRepository
    {
        Task<IEnumerable<ItemReferencesReport>> GetItemReferencesReportDataAsync(ItemReferencesReportFilter filter, CancellationToken ct = default);
    }
}
