﻿using Aldebaran.Application.Services.Models;

namespace Aldebaran.Application.Services
{
    public interface IDashBoardService
    {
        Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default);
        Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default);
        Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default);
        Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<NotificationWithError>> GetNotificationsWithError(string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<OutOfStockArticle>> GetOutOfStockAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<MinimumQuantityArticle>> GetMinimumQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrderNotification>> GetNotificationsByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default);
        Task<IEnumerable<MinimumLocalWarehouseQuantityArticle>> GetMinimumLocalWarehouseQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<LocalWarehouseAlarm>> GetLocalWarehouseAlarm(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<ConfirmedPurchaseOrder>> GetConfirmedPurchasesWithAutomaticAssigment(int employeeId, string? searchKey = null, CancellationToken ct = default);
        Task<IEnumerable<AutomaticCustomerOrder>> GetAutomaticCustomerOrdersAssigment(int processId, CancellationToken ct = default);
        Task<IEnumerable<AutomaticCustomerOrderDetail>> GetAutomaticCustomerOrderDetailsAssigment(int processOrderId, CancellationToken ct = default);

    }
}
