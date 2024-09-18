using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Services
{
    public class VisualizedMinimumQuantityAlarmService: IVisualizedMinimumQuantityAlarmService
    {
        private readonly IVisualizedMinimumQuantityAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedMinimumQuantityAlarmService(IVisualizedMinimumQuantityAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedMinimumQuantityAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(VisualizedMinimumQuantityAlarm visualizedAlarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedMinimumQuantityAlarm>(visualizedAlarm) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
