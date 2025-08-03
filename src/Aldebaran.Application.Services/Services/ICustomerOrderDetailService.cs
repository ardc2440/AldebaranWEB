using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderDetailService
    {
        Task<IEnumerable<CustomerOrderDetail>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default);
        Task<CustomerOrderDetail?> FindAsync(int customerOrderDetailId, CancellationToken ct = default);
    }

}
