using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerReservationService
    {
        Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default);
        Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, Reason? reason, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation> customerReservations, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);
    }
}
