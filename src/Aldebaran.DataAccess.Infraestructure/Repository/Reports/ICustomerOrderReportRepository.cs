using Aldebaran.DataAccess.Entities.Reports;

namespace Aldebaran.DataAccess.Infraestructure.Repository.Reports
{
    public interface ICustomerOrderReportRepository
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<BackOrderReport>> GetBackOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<BackOrderExport>> GetBackOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);
    }
}
