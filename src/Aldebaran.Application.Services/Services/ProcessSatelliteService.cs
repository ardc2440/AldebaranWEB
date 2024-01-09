using Aldebaran.Application.Services.Models;
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

        public async Task<ProcessSatellite?> FindAsync(int processSatelliteId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(processSatelliteId, ct);
            return _mapper.Map<ProcessSatellite?>(data);
        }

        public async Task<IEnumerable<ProcessSatellite>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<ProcessSatellite>>(data);
        }
    }

}
