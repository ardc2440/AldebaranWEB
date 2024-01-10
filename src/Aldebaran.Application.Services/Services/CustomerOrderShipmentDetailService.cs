using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderShipmentDetailService : ICustomerOrderShipmentDetailService
    {
        private readonly ICustomerOrderShipmentDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderShipmentDetailService(ICustomerOrderShipmentDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderShipmentDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerOrderShipmentDetail>> GetByCustomerOrderShipmentIdAsync(int customerOrderShipmentId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderShipmentIdAsync(customerOrderShipmentId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderShipmentDetail>>(data);
        }
    }

}
