using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ActivityTypesAreaService : IActivityTypesAreaService
    {
        private readonly IActivityTypesAreaRepository _repository;
        private readonly IMapper _mapper;
        public ActivityTypesAreaService(IActivityTypesAreaRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IActivityTypesAreaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByActivityTypeAsync(short activityTypeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByActivityTypeAsync(activityTypeId, ct);
            return _mapper.Map<IEnumerable<ActivityTypesArea>>(data);
        }

        public async Task<IEnumerable<ActivityTypesArea>> GetByAreaAsync(short areaId, CancellationToken ct = default)
        {
            var data = await _repository.GetByAreaAsync(areaId, ct);
            return _mapper.Map<IEnumerable<ActivityTypesArea>>(data);
        }
    }

}
