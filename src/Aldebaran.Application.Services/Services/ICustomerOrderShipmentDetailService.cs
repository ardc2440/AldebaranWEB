using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderShipmentDetailService
    {
        Task<IEnumerable<CustomerOrderShipmentDetail>> GetByCustomerOrderShipmentIdAsync(int customerOrderShipmentId, CancellationToken ct = default);
    }

}
