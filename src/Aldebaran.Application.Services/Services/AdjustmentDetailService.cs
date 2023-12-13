using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AdjustmentDetailService : IAdjustmentDetailService
    {
        private readonly IAdjustmentDetailRepository _repository;
        private readonly IMapper _mapper;
        public AdjustmentDetailService(IAdjustmentDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAdjustmentDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
