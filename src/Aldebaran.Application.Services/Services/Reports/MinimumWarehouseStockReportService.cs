using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class MinimumWarehouseStockReportService : IMinimumWarehouseStockReportService
    {
        private readonly IMinimumWarehouseStockReportRepository _repository;
        private readonly IMapper _mapper;

        public MinimumWarehouseStockReportService(IMinimumWarehouseStockReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IMinimumWarehouseStockReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetMinimumWarehouseStockReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<MinimumWarehouseStockReport>>(data);
        }
    }
}