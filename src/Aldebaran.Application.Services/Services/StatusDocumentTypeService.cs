using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class StatusDocumentTypeService : IStatusDocumentTypeService
    {
        private readonly IStatusDocumentTypeRepository _repository;
        private readonly IMapper _mapper;
        public StatusDocumentTypeService(IStatusDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IStatusDocumentTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
