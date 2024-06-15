using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IDashBoardService
    {
        Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int statusOrder, int? referenceId = null, CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(CancellationToken ct = default);
        Task<IEnumerable<ItemReference>> GetAllOutOfStockReferences(CancellationToken ct = default);
        Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(CancellationToken ct = default);
    }
}
