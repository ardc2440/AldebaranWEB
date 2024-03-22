using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class InProcessInventoryReportService : IInProcessInventoryReportService
    {
        private readonly IInProcessInventoryReportRepository _repository;
        private readonly IMapper _mapper;
        
        public InProcessInventoryReportService(IInProcessInventoryReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IInProcessInventoryReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<InProcessInventoryReport>> GetInProcessInventoryReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetInProcessInventoryReportDataAsync(ct);
            return _mapper.Map<IEnumerable<InProcessInventoryReport>>(data);
        }
    }
}
