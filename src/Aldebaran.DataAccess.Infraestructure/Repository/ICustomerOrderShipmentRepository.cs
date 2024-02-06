using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderShipmentRepository
    {
        Task<CustomerOrderShipment> AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct);
        Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct);
        Task UpdateAsync(int customerOrderShipmentId, CustomerOrderShipment customerOrderShipment, CancellationToken ct);
        Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderShipmentId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
    }

}
