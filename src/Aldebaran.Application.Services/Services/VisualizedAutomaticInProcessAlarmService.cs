using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Services
{
    public class VisualizedAutomaticInProcessAlarmService : IVisualizedAutomaticInProcessAlarmService
    {
        private readonly IVisualizedAutomaticInProcessAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedAutomaticInProcessAlarmService(IVisualizedAutomaticInProcessAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedAutomaticInProcessAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(VisualizedAutomaticInProcess visualizedAlarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedAutomaticInProcess>(visualizedAlarm) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
