using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ReferencesWarehouseService : IReferencesWarehouseService
    {
        private readonly IReferencesWarehouseRepository _repository;
        private readonly IMapper _mapper;
        public ReferencesWarehouseService(IReferencesWarehouseRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IReferencesWarehouseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var data = await _repository.GetByReferenceIdAsync(referenceId, ct);
            return _mapper.Map<IEnumerable<ReferencesWarehouse>>(data);

        }
    }

}
