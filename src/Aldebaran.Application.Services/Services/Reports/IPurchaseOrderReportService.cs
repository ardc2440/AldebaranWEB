using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IPurchaseOrderReportService
    {
        Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
