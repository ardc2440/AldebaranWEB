using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class WarehouseTransferDetailService : IWarehouseTransferDetailService
    {
        private readonly IWarehouseTransferDetailRepository _repository;
        private readonly IMapper _mapper;
        public WarehouseTransferDetailService(IWarehouseTransferDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IWarehouseTransferDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<WarehouseTransferDetail>> GetByWarehouseTransferIdAsync(int warehouseTransferId, CancellationToken ct = default)
        {
            var data = await _repository.GetByWarehouseTransferIdAsync(warehouseTransferId, ct);
            return _mapper.Map<IEnumerable<WarehouseTransferDetail>>(data);
        }
    }
}
