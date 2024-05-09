using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderNotificationService : IPurchaseOrderNotificationService
    {
        private readonly IPurchaseOrderNotificationRepository _repository;
        private readonly IMapper _mapper;
        public PurchaseOrderNotificationService(IPurchaseOrderNotificationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderNotificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderNotification>(purchaseOrderNotification) ?? throw new ArgumentNullException("Notificación de la orden de compra no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct = default)
        {
            var entity = await _repository.GetByPurchaseOrderId(purchaseOrderId, ct);
            return _mapper.Map<List<PurchaseOrderNotification>>(entity);            
        }

        public async Task UpdateNotificationStatusAsync(int purchaseOrderNotificationId, bool status, string errorMessage, CancellationToken ct = default)
        {
            await _repository.UpdateNotificationStatusAsync(purchaseOrderNotificationId, status, errorMessage, ct);
        }
    }
}
