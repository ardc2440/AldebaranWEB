using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly ICountryRepository _repository;
        private readonly IMapper _mapper;

        public CountryService(ICountryRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICountryRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<Country?> FindAsync(int countryId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(countryId, ct);
            return _mapper.Map<Country?>(data);
        }

        public async Task<IEnumerable<Country>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Country>>(data);
        }
    }
}
