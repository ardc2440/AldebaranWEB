using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Infraestructure.Models;
namespace Aldebaran.Application.Services
{
    public class EmailNotificationProviderSettingsService : IEmailNotificationProviderSettingsService
    {
        private readonly IEmailNotificationProviderSettingsRepository _repository;
        private readonly IMapper _mapper;
        public EmailNotificationProviderSettingsService(IEmailNotificationProviderSettingsRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IEmailNotificationProviderSettingsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task<EmailNotificationProvider> GetAsync(string subject, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(subject, ct);
            return _mapper.Map<EmailNotificationProvider>(data);
        }

        public async Task UpdateAsync(string subject, EmailNotificationProvider provider, CancellationToken ct = default)
        {
            var data = _mapper.Map<Entities.EmailNotificationProvider>(provider);
            await _repository.UpdateAsync(subject, data, ct);
        }
    }
}
