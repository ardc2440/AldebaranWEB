using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class WarehouseTransferReportService : IWarehouseTransferReportService
    {
        private readonly IWarehouseTransferReportRepository _repository;
        private readonly IMapper _mapper;

        public WarehouseTransferReportService(IWarehouseTransferReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IWarehouseTransferReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(WarehouseTransferReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetWarehouseTransferReportDataAsync(_mapper.Map<DataAccess.Entities.Reports.WarehouseTransferReportFilter>(filter ?? new WarehouseTransferReportFilter()), ct);
            return _mapper.Map<IEnumerable<WarehouseTransferReport>>(data);
        }
    }
}
