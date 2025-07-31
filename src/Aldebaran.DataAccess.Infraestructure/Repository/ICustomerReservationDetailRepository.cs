using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerReservationDetailRepository
    {
        Task<CustomerReservationDetail?> FindAsync(int customerReservationDetailId, CancellationToken ct = default);

        Task<IEnumerable<CustomerReservationDetail>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default);
        
        Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default);
    }

}
