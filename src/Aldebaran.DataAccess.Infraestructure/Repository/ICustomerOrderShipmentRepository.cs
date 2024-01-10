using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderShipmentRepository
    {
        Task AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct);
        Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct);
        Task UpdateAsync(int customerOrderShipmentId, CustomerOrderShipment customerOrderShipment, CancellationToken ct);
        Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default);
    }

}
