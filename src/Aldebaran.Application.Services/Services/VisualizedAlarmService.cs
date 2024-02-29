using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class VisualizedAlarmService : IVisualizedAlarmService
    {
        private readonly IVisualizedAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedAlarmService(IVisualizedAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<VisualizedAlarm> AddAsync(VisualizedAlarm visualizedAlarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedAlarm>(visualizedAlarm) ?? throw new ArgumentNullException("Orden no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<VisualizedAlarm>(result);
        }
    }

}
