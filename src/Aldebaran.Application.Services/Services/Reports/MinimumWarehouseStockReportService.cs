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

        public async Task<IEnumerable<MinimumWarehouseStockReport>> GetMinimumWarehouseStockReportDataAsync(MinimumWarehouseStockReportFilter filter, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var data = await _repository.GetMinimumWarehouseStockReportDataAsync(_mapper.Map<DataAccess.Entities.Reports.MinimumWarehouseStockReportFilter>(filter), ct);
            return _mapper.Map<IEnumerable<MinimumWarehouseStockReport>>(data);
        }
    }
}