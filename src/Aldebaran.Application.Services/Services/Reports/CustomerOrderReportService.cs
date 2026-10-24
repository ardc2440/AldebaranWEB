using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class CustomerOrderReportService : ICustomerOrderReportService
    {
        private readonly ICustomerOrderReportRepository _repository;
        private readonly IMapper _mapper;

        public CustomerOrderReportService(ICustomerOrderReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerOrderExport>> GetCustomerOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerOrderExportDataAsync(ToDataFilter(filter), ct);
            return _mapper.Map<IEnumerable<CustomerOrderExport>>(data);
        }

        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerOrderReportDataAsync(ToDataFilter(filter), ct);
            return _mapper.Map<IEnumerable<CustomerOrderReport>>(data);
        }

        public async Task<IEnumerable<BackOrderExport>> GetBackOrderExportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetBackOrderExportDataAsync(ToDataFilter(filter), ct);
            return _mapper.Map<IEnumerable<BackOrderExport>>(data);
        }

        public async Task<IEnumerable<BackOrderReport>> GetBackOrderReportDataAsync(CustomerOrderReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetBackOrderReportDataAsync(ToDataFilter(filter), ct);
            return _mapper.Map<IEnumerable<BackOrderReport>>(data);
        }

        private DataAccess.Entities.Reports.CustomerOrderReportFilter ToDataFilter(CustomerOrderReportFilter filter) =>
            _mapper.Map<DataAccess.Entities.Reports.CustomerOrderReportFilter>(filter ?? new CustomerOrderReportFilter());
    }
}
