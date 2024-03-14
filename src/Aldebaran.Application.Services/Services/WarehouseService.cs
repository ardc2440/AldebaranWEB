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

        public async Task<IEnumerable<Warehouse>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<List<Warehouse>>(data);
        }

        public async Task<Warehouse?> FindAsync(short warehouseId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(warehouseId, ct);
            return _mapper.Map<Warehouse?>(data);
        }

        public async Task<Warehouse?> FindByCodeAsync(short warehouseCode, CancellationToken ct = default)
        {
            var data = await _repository.FindByCodeAsync(warehouseCode, ct);
            return _mapper.Map<Warehouse?>(data);
        }

    }

}
