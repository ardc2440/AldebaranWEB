using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly IAlarmRepository _repository;
        private readonly IMapper _mapper;
        public AlarmService(IAlarmRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAlarmRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
