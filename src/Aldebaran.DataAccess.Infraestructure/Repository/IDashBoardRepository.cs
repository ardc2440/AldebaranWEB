using Aldebaran.DataAccess.Entities;

namespace Aldebaran.DataAccess.Infraestructure.Repository
{
    public interface IDashBoardRepository
    {
        Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<NotificationWithError>> GetNotificationsWithError(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<OutOfStockArticle>> GetOutOfStockAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<MinimumQuantityArticle>> GetMinimumQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderNotification>> GetNotificationsByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default);
    }
}
