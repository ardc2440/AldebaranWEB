using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class AutomaticPurchaseOrderAssigmentReportService : IAutomaticPurchaseOrderAssigmentReportService
    {
        private readonly IAutomaticPurchaseOrderAssigmentReportRepository _repository;
        private readonly IMapper _mapper;

        public AutomaticPurchaseOrderAssigmentReportService(IAutomaticPurchaseOrderAssigmentReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAutomaticPurchaseOrderAssigmentReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<AutomaticCustomerOrderAssigmentReport>> GetAutomaticCustomerOrderAssigmentReportDataAsync(AutomaticAssigmentReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetAutomaticCustomerOrderAssigmentReportDataAsync(_mapper.Map<DataAccess.Entities.Reports.AutomaticAssigmentReportFilter>(filter ?? new AutomaticAssigmentReportFilter()), ct);
            return _mapper.Map<IEnumerable<AutomaticCustomerOrderAssigmentReport>>(data);
        }

        public async Task<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>> GetAutomaticPendingCustomerOrderInProcessReportDataAsync(AutomaticPendingCustomerOrderInProcessReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetAutomaticPendingCustomerOrderInProcessReportDataAsync(_mapper.Map<DataAccess.Entities.Reports.AutomaticPendingCustomerOrderInProcessReportFilter>(filter ?? new AutomaticPendingCustomerOrderInProcessReportFilter()), ct);
            return _mapper.Map<IEnumerable<AutomaticPendingCustomerOrderInProcessReport>>(data);
        }
    }
}
