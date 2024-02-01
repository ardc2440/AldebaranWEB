using Aldebaran.Application.Services.Models;
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

        public async Task<IEnumerable<CancellationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(documentTypeCode,ct);
            return _mapper.Map<List<CancellationReason>>(data);
        }
    }
}
