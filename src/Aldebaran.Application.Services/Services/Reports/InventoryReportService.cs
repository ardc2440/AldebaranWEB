using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class InventoryReportService : IInventoryReportService
    {
        private readonly IInventoryReportRepository _repository;
        private readonly IMapper _mapper;

        public InventoryReportService(IInventoryReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(string referenceIdsFilter, CancellationToken ct = default)
        {
            var data = await _repository.GetInventoryReportDataAsync(referenceIdsFilter, ct);
            return _mapper.Map<IEnumerable<InventoryReport>>(data);
        }
    }
}
