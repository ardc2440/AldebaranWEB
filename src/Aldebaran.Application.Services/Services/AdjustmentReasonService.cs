using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AdjustmentReasonService : IAdjustmentReasonService
    {
        private readonly IAdjustmentReasonRepository _repository;
        private readonly IMapper _mapper;
        public AdjustmentReasonService(IAdjustmentReasonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAdjustmentReasonRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
