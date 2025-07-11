﻿using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<PurchaseOrder> AddAsync(PurchaseOrder purchaseOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(purchaseOrder) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<PurchaseOrder>(result);
        }

        public async Task CancelAsync(int purchaseOrderId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CancelAsync(purchaseOrderId, mapReason, ct);
        }

        public async Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(purchaseOrder) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            await _repository.ConfirmAsync(purchaseOrderId, entity, ct);
        }

        public async Task<PurchaseOrder?> FindAsync(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(purchaseOrderId, ct);
            return _mapper.Map<PurchaseOrder?>(data);
        }

        public async Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, ct);
            return (_mapper.Map<List<PurchaseOrder>>(data), count);
        }

        public async Task<(IEnumerable<PurchaseOrder>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, searchKey, ct);
            return (_mapper.Map<List<PurchaseOrder>>(data), count);
        }

        public async Task<int> UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, Reason reason, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(purchaseOrder) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            var mapOrdersAffected = _mapper.Map<IEnumerable<Entities.CustomerOrderAffectedByPurchaseOrderUpdate>>(ordersAffected);
            var result = await _repository.UpdateAsync(purchaseOrderId, entity, mapReason, mapOrdersAffected, ct);
            return result;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetTransitByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var data = await _repository.GetTransitByReferenceIdAsync(referenceId, ct);
            return _mapper.Map<List<PurchaseOrder>>(data);
        }

        public async Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, DateTime newExpectedReceiptDate, IEnumerable<PurchaseOrderDetail> purchaseOrderDetails, CancellationToken ct = default)
        {
            var details = _mapper.Map<List<Entities.PurchaseOrderDetail>>(purchaseOrderDetails) ?? throw new ArgumentNullException("Orden debe contener detalles.");
            var data = await _repository.GetAffectedCustomerOrders(purchaseOrderId, newExpectedReceiptDate, details, ct);
            return _mapper.Map<List<CustomerOrderAffectedByPurchaseOrderUpdate>>(data.OrderBy(o => o.OrderNumber));
        }

        public async Task<IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate>> GetAffectedCustomerOrders(int purchaseOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetAffectedCustomerOrders(purchaseOrderId, ct);
            return _mapper.Map<List<CustomerOrderAffectedByPurchaseOrderUpdate>>(data.OrderBy(o => o.OrderNumber));
        }
        public async Task<(IEnumerable<PurchaseOrder> purchaseOrders, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default)
        {
            var (d, r) = await _repository.GetAsync(skip, take, filter, orderBy, ct);
            var data = _mapper.Map<IEnumerable<PurchaseOrder>>(d);
            return (data, r);
        }

        /* Logs */
        public async Task<(IEnumerable<ModifiedPurchaseOrder>, int count)> GetPurchaseOrderModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetPurchaseOrderModificationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<ModifiedPurchaseOrder>>(data), c);
        }
        public async Task<(IEnumerable<CanceledPurchaseOrder>, int count)> GetPurchaseOrderCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetPurchaseOrderCancellationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<CanceledPurchaseOrder>>(data), c);
        }
    }
}
