using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AlarmMessageService : IAlarmMessageService
    {
        private readonly IAlarmMessageRepository _repository;
        private readonly IMapper _mapper;
        public AlarmMessageService(IAlarmMessageRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAlarmMessageRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<AlarmMessage>> GetByDocumentTypeIdAsync(short documentTypeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByDocumentTypeIdAsync(documentTypeId, ct);
            return _mapper.Map<List<AlarmMessage>>(data);
        }
    }

}
