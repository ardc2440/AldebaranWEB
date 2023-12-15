using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IWarehouseRepository _repository;
        private readonly IMapper _mapper;
        public WarehouseService(IWarehouseRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IWarehouseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<Warehouse>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<Warehouse>>(data);
        }

        public async Task<IEnumerable<Warehouse>> GetAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(filter, ct);
            return _mapper.Map<List<Warehouse>>(data);
        }

        public async Task<Warehouse?> FindAsync(int wareHouseId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(wareHouseId, ct);
            return _mapper.Map<Warehouse?>(data);
        }
    }

}
