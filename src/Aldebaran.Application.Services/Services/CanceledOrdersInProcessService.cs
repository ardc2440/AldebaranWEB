using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CanceledOrdersInProcessService : ICanceledOrdersInProcessService
    {
        private readonly ICanceledOrdersInProcessRepository _repository;
        private readonly IMapper _mapper;
        public CanceledOrdersInProcessService(ICanceledOrdersInProcessRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICanceledOrdersInProcessRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
