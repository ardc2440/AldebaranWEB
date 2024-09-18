using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services.Services
{
    public class VisualizedOutOfStockInventoryAlarmService: IVisualizedOutOfStockInventoryAlarmService
    {
        private readonly IVisualizedOutOfStockInventoryAlarmRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedOutOfStockInventoryAlarmService(IVisualizedOutOfStockInventoryAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedOutOfStockInventoryAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(VisualizedOutOfStockInventoryAlarm visualizedAlarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedOutOfStockInventoryAlarm>(visualizedAlarm) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }
    }
}
