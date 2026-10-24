using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface IAutomaticPurchaseOrderAssigmentReportService
    {
        Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(AutomaticAssigmentReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>> GetAutomaticPendingCustomerOrderInProcessReportDataAsync(AutomaticPendingCustomerOrderInProcessReportFilter filter, CancellationToken ct = default);
    }
}
