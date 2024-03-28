using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerReservationReportService
    {
        Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(string filter = "", CancellationToken ct = default);
    }
}
