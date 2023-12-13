using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ShippingMethodService : IShippingMethodService
    {
        private readonly IShippingMethodRepository _repository;
        private readonly IMapper _mapper;
        public ShippingMethodService(IShippingMethodRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IShippingMethodRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
