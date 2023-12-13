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
    }

}
