using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerOrderReportService
    {
        Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(string filter = "", CancellationToken ct = default);

        Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(string filter, CancellationToken ct = default);
    }
}
