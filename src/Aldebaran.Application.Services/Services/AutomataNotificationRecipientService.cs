using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class AutomataNotificationRecipientService : IAutomataNotificationRecipientService
    {
        private readonly IAutomataNotificationRecipientRepository _repository;
        private readonly IMapper _mapper;

        public AutomataNotificationRecipientService(IAutomataNotificationRecipientRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AutomataNotificationRecipient> CreateAsync(AutomataNotificationRecipient model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.AutomataNotificationRecipient>(model);
            var result = await _repository.CreateAsync(entity, ct);
            return _mapper.Map<AutomataNotificationRecipient>(result);
        }

        public async Task<IEnumerable<AutomataNotificationRecipient>> GetAllAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<AutomataNotificationRecipient>>(data);
        }

        public async Task<AutomataNotificationRecipient> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var data = await _repository.GetByIdAsync(id, ct);
            return _mapper.Map<AutomataNotificationRecipient>(data);
        }

        public async Task<AutomataNotificationRecipient> UpdateAsync(AutomataNotificationRecipient model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.AutomataNotificationRecipient>(model);
            var result = await _repository.UpdateAsync(entity, ct);
            return _mapper.Map<AutomataNotificationRecipient>(result);
        }
    }
}
