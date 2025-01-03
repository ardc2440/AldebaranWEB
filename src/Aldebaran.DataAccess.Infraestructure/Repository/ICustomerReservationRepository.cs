using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerReservationRepository
    {
        Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default);
        Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, Reason? reason, CancellationToken ct = default);
        Task<(IEnumerable<CustomerReservation> customerReservations, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);

        /* Logs */
        Task<(IEnumerable<ModifiedCustomerReservation>, int count)> GetCustomerReservationModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<(IEnumerable<CanceledCustomerReservation>, int count)> GetCustomerReservationCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
    }
}
