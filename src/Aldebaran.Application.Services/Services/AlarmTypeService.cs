using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AlarmTypeService : IAlarmTypeService
    {
        private readonly IAlarmTypeRepository _repository;
        private readonly IMapper _mapper;
        public AlarmTypeService(IAlarmTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAlarmTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<AlarmType>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<AlarmType>>(data);
        }
        public async Task<AlarmType?> FindAsync(short alarmTypeId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(alarmTypeId, ct);
            return _mapper.Map<AlarmType?>(data);
        }
    }
}