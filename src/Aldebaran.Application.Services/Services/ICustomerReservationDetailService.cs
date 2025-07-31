using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationDetailService
    {
        Task<CustomerReservationDetail?> FindAsync(int customerReservationDetailId, CancellationToken ct = default);

        Task<IEnumerable<CustomerReservationDetail>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default);

        Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default);
    }

}
