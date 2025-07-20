using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface IAutomaticPurchaseOrderAssigmentReportRepository
    {
        Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(string filter, CancellationToken ct = default);
        Task<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>> GetAutomaticPendingCustomerOrderInProcessReportDataAsync(string filter, CancellationToken ct = default);
    }
}
