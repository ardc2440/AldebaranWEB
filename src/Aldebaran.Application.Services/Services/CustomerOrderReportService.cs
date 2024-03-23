using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
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

        public async Task<IEnumerable<CustomerOrderReport>> GetCustomerOrderReportDataAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetCustomerOrderReportDataAsync(ct);
            return _mapper.Map<IEnumerable<CustomerOrderReport>>(data);
        }
    }
}
