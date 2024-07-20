using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class AdjustmentService : IAdjustmentService
    {
        private readonly IAdjustmentRepository _repository;
        private readonly IMapper _mapper;
        public AdjustmentService(IAdjustmentRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAdjustmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<Adjustment> AddAsync(Adjustment adjustment, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Adjustment>(adjustment) ?? throw new ArgumentNullException("Ajuste no puede ser nulo.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<Adjustment>(result);
        }

        public async Task CancelAsync(int adjustmentId, CancellationToken ct = default)
        {
            await _repository.CancelAsync(adjustmentId, ct);
        }

        public async Task<Adjustment?> FindAsync(int adjustmentId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(adjustmentId, ct);
            return _mapper.Map<Adjustment?>(data);
        }

        public async Task<(IEnumerable<Adjustment>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            var (data,count) = await _repository.GetAsync(skip, top, ct);
            return (_mapper.Map<List<Adjustment>>(data),count);
        }

        public async Task<(IEnumerable<Adjustment>, int)> GetAsync(int skip, int top, string filter, CancellationToken ct = default)
        {
            var (data,count) = await _repository.GetAsync(skip, top, filter, ct);
            return (_mapper.Map<List<Adjustment>>(data), count);
        }

        public async Task UpdateAsync(int adjustmentId, Adjustment adjustment, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Adjustment>(adjustment) ?? throw new ArgumentNullException("Ajuste no puede ser nulo.");
            await _repository.UpdateAsync(adjustmentId, entity, ct);
        }
    }
}
