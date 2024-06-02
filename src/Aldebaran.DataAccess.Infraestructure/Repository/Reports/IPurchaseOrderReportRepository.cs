using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IPurchaseOrderReportRepository
    {
        Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
