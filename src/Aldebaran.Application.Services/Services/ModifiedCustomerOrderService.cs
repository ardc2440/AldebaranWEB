using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModifiedCustomerOrderService : IModifiedCustomerOrderService
    {
        private readonly IModifiedCustomerOrderRepository _repository;
        private readonly IMapper _mapper;
        public ModifiedCustomerOrderService(IModifiedCustomerOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModifiedCustomerOrderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
