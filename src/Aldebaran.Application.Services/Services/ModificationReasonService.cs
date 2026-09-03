using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ModificationReasonService : IModificationReasonService
    {
        private readonly IModificationReasonRepository _repository;
        private readonly IMapper _mapper;
        public ModificationReasonService(IModificationReasonRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IModificationReasonRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<ModificationReason>> GetAsync(string documentTypeCode, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(documentTypeCode, ct);
            return _mapper.Map<List<ModificationReason>>(data);
        }

        public async Task<ModificationReason?> GetByDocumentAndNameAsync(string documentTypeCode, string name, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(documentTypeCode, ct);
                        
            return _mapper.Map<ModificationReason>(data.FirstOrDefault(d => d.ModificationReasonName == name));
        }
    }

}
