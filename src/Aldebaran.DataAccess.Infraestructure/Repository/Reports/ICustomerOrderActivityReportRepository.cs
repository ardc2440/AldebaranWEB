

using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerOrderActivityReportRepository
    {
        Task<IEnumerable<CustomerOrderActivityReport>> GetCustomerOrderActivityReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);
    }
}
