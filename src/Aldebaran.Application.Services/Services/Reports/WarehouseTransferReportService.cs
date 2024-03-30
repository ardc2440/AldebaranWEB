using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<WarehouseTransferReport>> GetWarehouseTransferReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            var data = await _repository.GetWarehouseTransferReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<WarehouseTransferReport>>(data);
        }
    }
}
