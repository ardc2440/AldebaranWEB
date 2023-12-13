using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModificationReasonService : IModificationReasonService
    {
        private readonly IModificationReasonRepository _repository;
        private readonly IMapper _mapper;
        public ModificationReasonService(IModificationReasonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModificationReasonRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
