using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AdjustmentTypeService : IAdjustmentTypeService
    {
        private readonly IAdjustmentTypeRepository _repository;
        private readonly IMapper _mapper;
        public AdjustmentTypeService(IAdjustmentTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAdjustmentTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
    }

}
