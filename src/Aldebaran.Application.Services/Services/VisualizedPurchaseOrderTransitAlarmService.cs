using Aldebaran.Application.Services.Models;
using Entities=Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class VisualizedPurchaseOrderTransitAlarmService : IVisualizedPurchaseOrderTransitAlarmService
    {
        private readonly IVisualizedPurchaseOrderTransitAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedPurchaseOrderTransitAlarmService(IVisualizedPurchaseOrderTransitAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedPurchaseOrderTransitAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task AddAsync(VisualizedPurchaseOrderTransitAlarm item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedPurchaseOrderTransitAlarm>(item) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);            
        }
    }
}
