using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;


namespace Aldebaran.Application.Services.Reports
{
    public class CustomerSaleReportService : ICustomerSaleReportService
    {
        private readonly ICustomerSaleReportRepository _repository;
        private readonly IMapper _mapper;

        public CustomerSaleReportService(ICustomerSaleReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerSaleReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerSaleReport>> GetCustomerSaleReportDataAsync(string filter = "", CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerSaleReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<CustomerSaleReport>>(data);
        }
    }
}
