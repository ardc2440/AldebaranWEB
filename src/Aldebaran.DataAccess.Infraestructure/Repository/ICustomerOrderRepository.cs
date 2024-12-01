using Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Models;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface ICustomerOrderRepository
    {
        Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, short editMode, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, short editMode, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, string searchKey, short editMode, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, string searchKey, short editMode, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default);        
    }
}
