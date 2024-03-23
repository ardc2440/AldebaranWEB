using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class WarehouseStockReportService : IWarehouseStockReportService
    {
        private readonly IWarehouseStockReportRepository _repository;
        private readonly IMapper _mapper;

        public WarehouseStockReportService(IWarehouseStockReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IWarehouseStockReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetWarehouseStockReportDataAsync(ct);
            return _mapper.Map<IEnumerable<WarehouseStockReport>>(data);
        }
    }
}
