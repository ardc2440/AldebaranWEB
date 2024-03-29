using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
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

        public async Task<IEnumerable<WarehouseStockReport>> GetWarehouseStockReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetWarehouseStockReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<WarehouseStockReport>>(data);
        }
    }
}
