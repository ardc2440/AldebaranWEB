using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerOrderReportService
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<BackOrderReport>> GetBackOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);

        Task<IEnumerable<BackOrderExport>> GetBackOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default);
    }
}
