using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderRepository
    {
        Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(string searchKey, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        CustomerOrder? Find(int customerOrderId);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default);
    }
}
