using Aldebaran.Application.Services.Models;
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

        public async Task<ShippingMethod?> FindAsync(short ShippingMethodId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(ShippingMethodId, ct);
            return _mapper.Map<ShippingMethod?>(data);
        }

        public async Task<IEnumerable<ShippingMethod>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<ShippingMethod>>(data);
        }
    }

}
