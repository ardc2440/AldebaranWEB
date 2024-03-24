using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationReportService
    {
        Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CancellationToken ct = default);
    }
}
