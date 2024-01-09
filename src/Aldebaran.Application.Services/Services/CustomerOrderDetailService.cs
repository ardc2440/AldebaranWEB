using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderDetailService : ICustomerOrderDetailService
    {
        private readonly ICustomerOrderDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerOrderDetailService(ICustomerOrderDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerOrderDetail>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderIdAsync(customerOrderId, ct);
            return _mapper.Map<IEnumerable<CustomerOrderDetail>>(data);
        }
    }

}
