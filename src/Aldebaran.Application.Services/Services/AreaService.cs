using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
namespace Aldebaran.Application.Services
{
    public class AreaService : IAreaService
    {
        private readonly IAreaRepository _repository;
        private readonly IMapper _mapper;

        public AreaService(IAreaRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAreaRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<Area?> FindAsync(short areaId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(areaId, ct);
            return _mapper.Map<Area?>(data);
        }

        public async Task<IEnumerable<Area>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Area>>(data);
        }

        public async Task<IEnumerable<Area>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Area>>(data);
        }
    }
}
