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
        public async Task<IEnumerable<CustomerReservationReport>> GetCustomerReservationReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerReservationReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<CustomerReservationReport>>(data);
        }
    }
}
