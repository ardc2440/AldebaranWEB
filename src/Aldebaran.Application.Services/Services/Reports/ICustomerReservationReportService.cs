using Aldebaran.Application.Services.Models.Reports;

namespace Aldebaran.Application.Services.Reports
{
    public interface ICustomerReservationReportService
    {
        Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CancellationToken ct = default);
    }
}
