using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class AutomataConnectivityErrorPatternService : IAutomataConnectivityErrorPatternService
    {
        private readonly IAutomataConnectivityErrorPatternRepository _repository;
        private readonly IMapper _mapper;

        public AutomataConnectivityErrorPatternService(IAutomataConnectivityErrorPatternRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AutomataConnectivityErrorPattern> CreateAsync(AutomataConnectivityErrorPattern model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.AutomataConnectivityErrorPattern>(model);
            var result = await _repository.CreateAsync(entity, ct);
            return _mapper.Map<AutomataConnectivityErrorPattern>(result);
        }

        public async Task<IEnumerable<AutomataConnectivityErrorPattern>> GetAllAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<AutomataConnectivityErrorPattern>>(data);
        }

        public async Task<AutomataConnectivityErrorPattern> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var data = await _repository.GetByIdAsync(id, ct);
            return _mapper.Map<AutomataConnectivityErrorPattern>(data);
        }

        public async Task<AutomataConnectivityErrorPattern> UpdateAsync(AutomataConnectivityErrorPattern model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.AutomataConnectivityErrorPattern>(model);
            var result = await _repository.UpdateAsync(entity, ct);
            return _mapper.Map<AutomataConnectivityErrorPattern>(result);
        }
    }
}
