using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repository;
        private readonly IPurchaseOrderApprovalRangeService _purchaseOrderApprovalRangeService;
        private readonly IStatusDocumentTypeService _statusDocumentTypeService;
        private readonly IModificationReasonService _modificationReasonService;
        private readonly IDocumentTypeService _documentTypeService;
        private readonly IMapper _mapper;

        public PurchaseOrderService(
            IPurchaseOrderRepository repository,
            IPurchaseOrderApprovalRangeService purchaseOrderApprovalRangeService,
            IStatusDocumentTypeService statusDocumentTypeService,
            IModificationReasonService modificationReasonService,
            IDocumentTypeService documentTypeService,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderRepository));
            _purchaseOrderApprovalRangeService = purchaseOrderApprovalRangeService ?? throw new ArgumentNullException(nameof(IPurchaseOrderApprovalRangeService));
            _statusDocumentTypeService = statusDocumentTypeService ?? throw new ArgumentNullException(nameof(IStatusDocumentTypeService));
            _modificationReasonService = modificationReasonService ?? throw new ArgumentNullException(nameof(IModificationReasonService));
            _documentTypeService = documentTypeService ?? throw new ArgumentNullException(nameof(IDocumentTypeService));
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

        public async Task ConfirmAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, int? approvalEmployeeId = null, string? approvalReason = null, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrder>(purchaseOrder) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            await _repository.ConfirmAsync(purchaseOrderId, entity, approvalEmployeeId, approvalReason, ct);
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

        public async Task<PurchaseOrderConfirmationValidationResult> ValidateConfirmationAsync(PurchaseOrder purchaseOrder, CancellationToken ct = default)
        {
            foreach (var detail in purchaseOrder.PurchaseOrderDetails)
            {
                if (detail.RequestedQuantity != detail.ReceivedQuantity)
                {
                    var evaluation = await _purchaseOrderApprovalRangeService.EvaluateAdjustmentAsync(detail.RequestedQuantity, detail.ReceivedQuantity ?? 0, ct);

                    if (evaluation.RequiresApproval)
                        return new PurchaseOrderConfirmationValidationResult() { RequiresApproval = true };
                }
            }

            return new PurchaseOrderConfirmationValidationResult() { RequiresApproval = false };
        }

        public async Task<bool> RequestApprovalAsync(int purchaseOrderId, PurchaseOrder purchaseOrder, int employeeId, CancellationToken ct = default)
        {
            var documentType = await _documentTypeService.FindByCodeAsync("O", ct) ?? throw new Exception("Tipo de documento no encontrado");
            var statusDocumentTypeId = await _statusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 4, ct) ?? throw new Exception("Estado de documento no encontrado");
            var adjustmentReason = await _modificationReasonService.GetByDocumentAndNameAsync("O", "Solicitud aprobación ajustes", ct) ?? throw new Exception("Motivo de modificación no encontrado");

            var reason = new Reason { ReasonId = adjustmentReason.ModificationReasonId, EmployeeId = employeeId };
            purchaseOrder.StatusDocumentTypeId = statusDocumentTypeId.StatusDocumentTypeId;

            var result = await UpdateAsync(purchaseOrderId, purchaseOrder, reason, new List<CustomerOrderAffectedByPurchaseOrderUpdate>(), ct: ct);

            return result > 0;
        }

        public async Task<bool> DenyApprovalAsync(int purchaseOrderId, int employeeId, string denyReason, CancellationToken ct = default)
        {
            var documentType = await _documentTypeService.FindByCodeAsync("O", ct) ?? throw new Exception("Tipo de documento no encontrado");
            var statusDocumentTypeId = await _statusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1, ct) ?? throw new Exception("Estado de documento no encontrado");

            var denyApproval = new Entities.PurchaseOrderAdjustmentLog
            {
                PurchaseOrderId = purchaseOrderId,
                EmployeeId = employeeId,
                NewStatusDocumentTypeId = statusDocumentTypeId.StatusDocumentTypeId,
                Reason = denyReason,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _repository.DenyAdjustmentApproval(denyApproval, ct);

            return result > 0;
        }
    }
}
