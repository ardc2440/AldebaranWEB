using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IAutomaticPurchaseOrderAssigmentReportRepository
    {
        Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(AutomaticAssigmentReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>> GetAutomaticPendingCustomerOrderInProcessReportDataAsync(AutomaticPendingCustomerOrderInProcessReportFilter filter, CancellationToken ct = default);
    }
}
