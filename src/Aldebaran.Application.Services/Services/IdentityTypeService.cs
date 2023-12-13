using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class IdentityTypeService : IIdentityTypeService
    {
        private readonly IIdentityTypeRepository _repository;
        private readonly IMapper _mapper;
        public IdentityTypeService(IIdentityTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IIdentityTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
