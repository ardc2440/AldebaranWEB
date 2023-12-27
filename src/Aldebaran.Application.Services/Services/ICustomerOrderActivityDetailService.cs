using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderActivityDetailService
    {
        Task DeleteAsync(int customerOrderActivityDetailId, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderActivityDetail>> GetAsync(int customerOrderActivityId, CancellationToken ct = default);

    }

}
