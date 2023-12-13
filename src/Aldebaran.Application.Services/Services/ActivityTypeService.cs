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
    }

}
