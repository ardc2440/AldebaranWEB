using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderDetailService
    {
        Task<IEnumerable<CustomerOrderDetail>> GetAsync(int customerOrderId, CancellationToken ct = default);
    }

}
