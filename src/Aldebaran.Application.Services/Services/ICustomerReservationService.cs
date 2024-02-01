using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationService
    {
        Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservation>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<CustomerReservation>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default);
        Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, CancellationToken ct = default);
    }
}
