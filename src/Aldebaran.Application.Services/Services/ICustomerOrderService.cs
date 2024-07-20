using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderService
    {
        Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default);
    }

}
