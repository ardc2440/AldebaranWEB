using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _repository;
        private readonly IMapper _mapper;

        public CityService(ICityRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICityRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<City?> FindAsync(int cityId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(cityId, ct);
            return _mapper.Map<City?>(data);
        }

        public async Task<IEnumerable<City>> GetByDepartmentIdAsync(int departmentId, CancellationToken ct = default)
        {
            var data = await _repository.GetByDepartmentIdAsync(departmentId, ct);
            return _mapper.Map<List<City>>(data);
        }
    }
}
