using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IDashBoardRepository _repository;
        private readonly IMapper _mapper;

        public DashBoardService(IDashBoardRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IDashBoardRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<IEnumerable<CustomerReservation>> GetExpiredReservationsAsync(string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetExpiredReservationsAsync(searchKey, ct);
            return _mapper.Map<IEnumerable<CustomerReservation>>(data);
        }
        public async Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByEmployeeIdAsync(employeeId, ct);
            return _mapper.Map<List<Alarm>>(data);
        }
        public async Task<Employee?> FindByLoginUserIdAsync(string loginUserId, CancellationToken ct = default)
        {
            var data = await _repository.FindByLoginUserIdAsync(loginUserId, ct);
            return _mapper.Map<Employee?>(data);
        }
        public async Task<Models.DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            var data = await _repository.FindByCodeAsync(code, ct);
            return _mapper.Map<Models.DocumentType?>(data);
        }
        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            var data = await _repository.FindByDocumentAndOrderAsync(documentTypeId, order, ct);
            return _mapper.Map<StatusDocumentType?>(data);
        }
        public async Task<IEnumerable<PurchaseOrderTransitAlarm>> GetAllTransitAlarmAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetAllTransitAlarmAsync(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderTransitAlarm>>(data);
        }
        public async Task<IEnumerable<PurchaseOrder>> GetPurchaseOrderExpirationsAsync(int purchaseOrderWitheFlag, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetPurchaseOrderExpirationsAsync(purchaseOrderWitheFlag, searchKey, ct);
            return _mapper.Map<IEnumerable<PurchaseOrder>>(data);
        }
        public async Task<IEnumerable<CustomerOrder>> GetExpiredCustomerOrdersAsync(string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetExpiredCustomerOrdersAsync(searchKey, ct);
            return _mapper.Map<IEnumerable<CustomerOrder>>(data);
        }
        public async Task<IEnumerable<NotificationWithError>> GetNotificationsWithError(string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetNotificationsWithError(searchKey, ct);
            return _mapper.Map<List<NotificationWithError>>(data.OrderBy(o => o.NotificationDate));
        }
        public async Task<IEnumerable<OutOfStockArticle>> GetOutOfStockAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetOutOfStockAlarmsAsync(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<OutOfStockArticle>>(data);
        }
        public async Task<IEnumerable<MinimumQuantityArticle>> GetMinimumQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetMinimumQuantityAlarmsAsync(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<MinimumQuantityArticle>>(data);
        }
        public async Task<IEnumerable<PurchaseOrderNotification>> GetNotificationsByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default)
        {
            var entity = await _repository.GetNotificationsByModifiedPurchaseOrder(modifiedPurchaseOrderId, ct);
            return _mapper.Map<List<PurchaseOrderNotification>>(entity);
        }
        public async Task<IEnumerable<MinimumLocalWarehouseQuantityArticle>> GetMinimumLocalWarehouseQuantityAlarmsAsync(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetMinimumLocalWarehouseQuantityAlarmsAsync(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<MinimumLocalWarehouseQuantityArticle>>(data);
        }

        public async Task<IEnumerable<LocalWarehouseAlarm>> GetLocalWarehouseAlarm(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetLocalWarehouseAlarmAsync(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<LocalWarehouseAlarm>>(data);
        }
        public async Task<IEnumerable<ConfirmedPurchaseOrder>> GetConfirmedPurchasesWithAutomaticAssigment(int employeeId, string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetConfirmedPurchasesWithAutomaticAssigment(employeeId, searchKey, ct);
            return _mapper.Map<IEnumerable<ConfirmedPurchaseOrder>>(data);
        }
        public async Task<IEnumerable<AutomaticCustomerOrder>> GetAutomaticCustomerOrdersAssigment(int processId, CancellationToken ct = default)
        {
            var data = await _repository.GetAutomaticCustomerOrdersAssigment(processId, ct);
            return _mapper.Map<IEnumerable<AutomaticCustomerOrder>>(data);
        }
        public async Task<IEnumerable<AutomaticCustomerOrderDetail>> GetAutomaticCustomerOrderDetailsAssigment(int processOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetAutomaticCustomerOrderDetailsAssigment(processOrderId, ct);
            return _mapper.Map<IEnumerable<AutomaticCustomerOrderDetail>>(data);
        }
    }
}
