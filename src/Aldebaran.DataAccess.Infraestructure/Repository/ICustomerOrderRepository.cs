using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderRepository
    {
        Task<CustomerOrder> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, CancellationToken ct = default);
    }
}
