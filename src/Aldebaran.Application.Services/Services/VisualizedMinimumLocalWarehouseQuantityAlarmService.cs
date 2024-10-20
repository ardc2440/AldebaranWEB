using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Services
{
    public class VisualizedMinimumLocalWarehouseQuantityAlarmService : IVisualizedMinimumLocalWarehouseQuantityAlarmService
    {
        private readonly IVisualizedMinimumLocalWarehouseQuantityAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedMinimumLocalWarehouseQuantityAlarmService(IVisualizedMinimumLocalWarehouseQuantityAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedMinimumLocalWarehouseQuantityAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(VisualizedMinimumLocalWarehouseQuantityAlarm visualizedAlarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedMinimumLocalWarehouseQuantityAlarm>(visualizedAlarm) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
