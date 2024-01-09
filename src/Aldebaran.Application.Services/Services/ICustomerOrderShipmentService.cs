using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderShipmentService
    {
        Task AddAsync(CustomerOrderShipment customerOrderShipment, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderShipment>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderInProcessId, CustomerOrderShipment customerOrderShipment, CancellationToken ct = default);
        Task<CustomerOrderShipment?> FindAsync(int customerOrderShipmentId, CancellationToken ct = default);
    }

}
