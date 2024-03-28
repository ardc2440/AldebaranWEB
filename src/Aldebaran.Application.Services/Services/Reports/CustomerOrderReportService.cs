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

        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerOrderReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<CustomerOrderReport>>(data);
        }
    }
}
