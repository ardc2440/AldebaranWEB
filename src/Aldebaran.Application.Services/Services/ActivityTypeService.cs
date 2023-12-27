using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IActivityTypeRepository _repository;
        private readonly IMapper _mapper;
        public ActivityTypeService(IActivityTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IActivityTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<ActivityType?> FindAsync(short activityTypeId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(activityTypeId, ct);
            return _mapper.Map<ActivityType?>(data);
        }
    }

}
