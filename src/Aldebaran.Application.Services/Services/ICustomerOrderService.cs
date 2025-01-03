using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderService
    {
        Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, short editMode = -1, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, string searchKey, short editMode = -1, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, short editMode = -1, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetWhitOutCancellationRequestAsync(int skip, int top, string searchKey, short editMode = -1, CancellationToken ct = default);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default);

        /* Logs */
        Task<(IEnumerable<ModifiedCustomerOrder>, int count)> GetCustomerOrderModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<(IEnumerable<CanceledCustomerOrder>, int count)> GetCustomerOrderCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<(IEnumerable<ClosedCustomerOrder>, int count)> GetCustomerOrderClosesLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
    }

}
