using Aldebaran.Application.Services.Models;
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

        public async Task<IEnumerable<AdjustmentReason>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<List<AdjustmentReason>>(data);
        }
    }

}
