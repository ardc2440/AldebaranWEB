using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderTransitAlarmService : IPurchaseOrderTransitAlarmService
    {
        private readonly IPurchaseOrderTransitAlarmRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseOrderTransitAlarmService(IPurchaseOrderTransitAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderTransitAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(PurchaseOrderTransitAlarm item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderTransitAlarm>(item) ?? throw new ArgumentNullException("Alarma de la orden de compra no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
