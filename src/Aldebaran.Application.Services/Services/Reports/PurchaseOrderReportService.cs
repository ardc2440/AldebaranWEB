using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class PurchaseOrderReportService : IPurchaseOrderReportService
    {
        private readonly IPurchaseOrderReportRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseOrderReportService(IPurchaseOrderReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<PurchaseOrderReport>> GetPurchaseOrderReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetPurchaseOrderReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<PurchaseOrderReport>>(data);
        }
    }
}
