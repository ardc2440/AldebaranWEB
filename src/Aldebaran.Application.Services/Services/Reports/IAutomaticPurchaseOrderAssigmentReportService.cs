using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IAutomaticPurchaseOrderAssigmentReportService
    {
        Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(string filter, CancellationToken ct = default);
    }
}
