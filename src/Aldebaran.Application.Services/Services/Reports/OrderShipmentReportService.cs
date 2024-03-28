using Aldebaran.Application.Services.Models.Reports;
using Aldebaran.DataAccess.Infraestructure.Repository.Reports;
using AutoMapper;

namespace Aldebaran.Application.Services.Reports
{
    public class OrderShipmentReportService : IOrderShipmentReportService
    {
        private readonly IOrderShipmentReportRepository _repository;
        private readonly IMapper _mapper;

        public OrderShipmentReportService(IOrderShipmentReportRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IOrderShipmentReportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<OrderShipmentReport>> GetOrderShipmentReportDataAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetOrderShipmentReportDataAsync(filter, ct);
            return _mapper.Map<IEnumerable<OrderShipmentReport>>(data);
        }
    }
}
