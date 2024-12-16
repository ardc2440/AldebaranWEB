using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services.Services
{
    public class VisualizedLocalWarehouseAlarmService : IVisualizedLocalWarehouseAlarmService
    {
        private readonly IVisualizedLocalWarehouseAlarmRepository _repository;
        private readonly IMapper _mapper;

        public VisualizedLocalWarehouseAlarmService(IVisualizedLocalWarehouseAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedLocalWarehouseAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(VisualizedLocalWarehouseAlarm item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedLocalWarehouseAlarm>(item) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
