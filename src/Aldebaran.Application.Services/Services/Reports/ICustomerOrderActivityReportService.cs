using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerOrderActivityReportService
    {
        Task<IEnumerable<CustomerOrderActivityReport>> GetCustomerOrderActivityReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
