using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModifiedOrdersInProcessService : IModifiedOrdersInProcessService
    {
        private readonly IModifiedOrdersInProcessRepository _repository;
        private readonly IMapper _mapper;
        public ModifiedOrdersInProcessService(IModifiedOrdersInProcessRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModifiedOrdersInProcessRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
