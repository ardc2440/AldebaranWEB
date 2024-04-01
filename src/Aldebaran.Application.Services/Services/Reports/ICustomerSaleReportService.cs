using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerSaleReportService
    {
        Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
