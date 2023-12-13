using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ItemReferenceService : IItemReferenceService
    {
        private readonly IItemReferenceRepository _repository;
        private readonly IMapper _mapper;
        public ItemReferenceService(IItemReferenceRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IItemReferenceRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
