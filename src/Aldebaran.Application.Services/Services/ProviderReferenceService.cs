using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ProviderReferenceService : IProviderReferenceService
    {
        private readonly IProviderReferenceRepository _repository;
        private readonly IMapper _mapper;
        public ProviderReferenceService(IProviderReferenceRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IProviderReferenceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
