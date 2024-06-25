using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IDashBoardService
    {
        Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int statusOrder, string? searchKey = null, int? referenceId = null, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllOutOfStockReferences(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(string? searchKey = null, CancellationToken ct = default);
    }
}
