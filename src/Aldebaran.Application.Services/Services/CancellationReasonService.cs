using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CancellationReasonService : ICancellationReasonService
    {
        private readonly ICancellationReasonRepository _repository;
        private readonly IMapper _mapper;
        public CancellationReasonService(ICancellationReasonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICancellationReasonRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
