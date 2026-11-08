using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IPurchaseOrderReportRepository
    {
        Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(PurchaseOrderReportFilter filter, CancellationToken ct = default);        
    }
}
