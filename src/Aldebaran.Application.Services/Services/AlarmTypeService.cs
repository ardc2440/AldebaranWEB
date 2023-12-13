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
    }

}
