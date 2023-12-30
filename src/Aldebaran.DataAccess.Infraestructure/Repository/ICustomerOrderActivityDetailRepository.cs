using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderActivityDetailRepository
    {
        Task DeleteAsync(int customerOrderActivityDetailId, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderActivityDetail>> GetByCustomerOrderActivityIdAsync(int customerOrderActivityId, CancellationToken ct = default);
    }

}
