using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services
{
    public class InventoryAdjustmentReportService: IInventoryAdjustmentReportService
    {
        private readonly IInventoryAdjustmentReportRepository _repository;
        private readonly IMapper _mapper;
        
        public InventoryAdjustmentReportService(IInventoryAdjustmentReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInventoryAdjustmentReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<InventoryAdjustmentReport>> GetInventoryAdjustmentReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetInventoryAdjustmentReportDataAsync(ct);
            return _mapper.Map<IEnumerable<InventoryAdjustmentReport>>(data);
        }
    }
}
