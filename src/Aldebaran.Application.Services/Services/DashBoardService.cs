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

        public async Task<IEnumerable<PurchaseOrderDetail>> GetTransitDetailOrdersAsync(int statusOrder, string? searchKey = null, int? referenceId = null, CancellationToken ct = default)
        {
            var data = await _repository.GetByReferenceIdAndStatusOrderAsync(statusOrder, searchKey, referenceId, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderDetail>>(data);
        }

        public async Task<IEnumerable<ItemReference>> GetAllReferencesWithMinimumQuantityAsync(string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetAllReferencesWithMinimumQuantityAsync(searchKey, ct);
            return _mapper.Map<List<ItemReference>>(data);
        }

        public async Task<IEnumerable<ItemReference>> GetAllOutOfStockReferences(string? searchKey = null, CancellationToken ct = default)
        {
            var data = await _repository.GetAllOutOfStockReferences(searchKey, ct);
            return _mapper.Map<List<ItemReference>>(data);
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
    }
}
