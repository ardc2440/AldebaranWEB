using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class WarehouseTransferService : IWarehouseTransferService
    {
        private readonly IWarehouseTransferRepository _repository;
        private readonly IMapper _mapper;
        public WarehouseTransferService(IWarehouseTransferRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IWarehouseTransferRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task CancelAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            await _repository.CancelAsync(warehouseTransferId, ct);
        }
        public async Task<WarehouseTransfer?> AddAsync(WarehouseTransfer warehouseTransfer, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.WarehouseTransfer>(warehouseTransfer);
            data = await _repository.AddAsync(data, ct);

            return _mapper.Map<WarehouseTransfer>(data);
        }

        public async Task<WarehouseTransfer?> FindAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(warehouseTransferId, ct);
            return _mapper.Map<WarehouseTransfer?>(data);
        }

        public async Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, ct);
            return (_mapper.Map<List<WarehouseTransfer>>(data), count);
        }

        public async Task<(IEnumerable<WarehouseTransfer>, int)> GetAsync(int skip, int top, string search, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, search, ct);
            return (_mapper.Map<List<WarehouseTransfer>>(data), count);
        }        
    }

}
