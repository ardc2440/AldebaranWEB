using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly INotificationTemplateRepository _repository;
        private readonly IMapper _mapper;
        public NotificationTemplateService(INotificationTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(INotificationTemplateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<NotificationTemplate?> FindAsync(string name, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(name, ct);
            return _mapper.Map<NotificationTemplate>(data);
        }

        public async Task<IEnumerable<NotificationTemplate>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<NotificationTemplate>>(data);
        }

        public async Task UpdateAsync(short templateId, NotificationTemplate template, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.NotificationTemplate>(template) ?? throw new ArgumentNullException("Plantilla no puede ser nula.");
            await _repository.UpdateAsync(templateId, entity, ct);
        }
    }
}
