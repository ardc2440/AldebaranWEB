using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ProcessSatelliteService : IProcessSatelliteService
    {
        private readonly IProcessSatelliteRepository _repository;
        private readonly IMapper _mapper;
        public ProcessSatelliteService(IProcessSatelliteRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProcessSatelliteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
