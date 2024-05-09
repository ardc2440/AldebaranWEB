using Aldebaran.Application.Services.Models;
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

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<PurchaseOrder>>(data);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<PurchaseOrder>>(data);
        }

        public async Task UpdateAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, Reason reason, IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> ordersAffected, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(purchaseOrder) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            var mapOrdersAffected = _mapper.Map<IEnumerable<Entities.CustomerOrderAffectedByPurchaseOrderUpdate>>(ordersAffected);
            await _repository.UpdateAsync(purchaseOrderId, entity, mapReason, mapOrdersAffected,ct);
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
            return _mapper.Map<List<CustomerOrderAffectedByPurchaseOrderUpdate>>(data);
        }
    }
}
