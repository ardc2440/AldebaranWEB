﻿using Aldebaran.Application.Services.Models;
namespace Aldebaran.Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default);
        Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, CancellationToken ct = default);
        Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default);
        Task<PurchaseOrder> AddAsync(PurchaseOrder purchaseOrder, CancellationToken ct = default);
        Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default);
        Task<int> UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, Reason reason, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, CancellationToken ct = default);
        Task<IEnumerable<PurchaseOrder>> GetTransitByReferenceIdAsync(int referenceId, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, DateTime newExpectedReceiptDate, IEnumerable<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken ct = default);
        Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, CancellationToken ct = default);
        Task<(IEnumerable<PurchaseOrder> purchaseOrders, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default);

        /* Logs */
        Task<(IEnumerable<ModifiedPurchaseOrder>, int count)> GetPurchaseOrderModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);
        Task<(IEnumerable<CanceledPurchaseOrder>, int count)> GetPurchaseOrderCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default);

    }
}
