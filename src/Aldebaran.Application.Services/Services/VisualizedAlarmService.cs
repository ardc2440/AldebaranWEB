using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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
    }

}
