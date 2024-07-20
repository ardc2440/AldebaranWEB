using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface ICustomerOrderService
    {
        Task<CustomerOrder?> AddAsync(CustomerOrder customerOrder, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, short editMode = -1, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder>, int count)> GetAsync(int skip, int top, string searchKey, short editMode = -1, CancellationToken ct = default);
        Task<CustomerOrder?> FindAsync(int customerOrderId, CancellationToken ct = default);
        Task CancelAsync(int customerOrderId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task CloseAsync(int customerOrderId, short closedStatusDocumentId, Reason reason, CancellationToken ct = default);
        Task UpdateAsync(int customerOrderId, CustomerOrder customerOrder, Reason reason, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder> customerOrders, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder> customerOrders, int count)> GetCustomerOrderShipmentAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);
        Task<(IEnumerable<CustomerOrder> customerOrders, int count)> GetCustomerOrderInProcessAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);
    }

}
