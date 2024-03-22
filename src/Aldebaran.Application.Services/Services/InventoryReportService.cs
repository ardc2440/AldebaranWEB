using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class InventoryReportService: IInventoryReportService
    {
        private readonly IInventoryReportRepository _repository;
        private readonly IMapper _mapper;

        public InventoryReportService(IInventoryReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInventoryReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<InventoryReport>> GetInventoryReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetInventoryReportDataAsync(ct);
            return _mapper.Map<IEnumerable<InventoryReport>>(data);
        }
    }
}
