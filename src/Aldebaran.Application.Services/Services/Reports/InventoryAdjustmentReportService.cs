using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class InventoryAdjustmentReportService : IInventoryAdjustmentReportService
    {
        private readonly IInventoryAdjustmentReportRepository _repository;
        private readonly IMapper _mapper;

        public InventoryAdjustmentReportService(IInventoryAdjustmentReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInventoryAdjustmentReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetInventoryAdjustmentReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<InventoryAdjustmentReport>>(data);
        }
    }
}
