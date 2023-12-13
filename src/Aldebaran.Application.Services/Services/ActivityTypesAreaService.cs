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
    }

}
