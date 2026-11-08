using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class CustomerReservationReportService : ICustomerReservationReportService
    {
        private readonly ICustomerReservationReportRepository _repository;
        private readonly IMapper _mapper;

        public CustomerReservationReportService(ICustomerReservationReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(CustomerReservationReportFilter filter, CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerReservationReportDataAsync(_mapper.Map<DataAccess.Entities.Reports.CustomerReservationReportFilter>(filter ?? new CustomerReservationReportFilter()), ct);
            return _mapper.Map<IEnumerable<CustomerReservationReport>>(data);
        }
    }
}
