using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ShipmentMethodService : IShipmentMethodService
    {
        private readonly IShipmentMethodRepository _repository;
        private readonly IMapper _mapper;

        public ShipmentMethodService(IShipmentMethodRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IShipmentMethodRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ShipmentMethod>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<ShipmentMethod>>(data);
        }
    }
}
