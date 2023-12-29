using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderInProcessDetailService
    {
        Task<IEnumerable<CustomerOrderInProcessDetail>> GetAsync(int customerOrderInProcessId, CancellationToken ct = default);
    }

}
