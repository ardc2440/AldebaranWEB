using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderShipmentDetailRepository
    {
        Task<IEnumerable<CustomerOrderShipmentDetail>> GetByCustomerOrderShipmentIdAsync(int customerOrderShipmentId, CancellationToken ct);
    }

}
