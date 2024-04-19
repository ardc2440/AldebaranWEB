using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class StatusDocumentTypeService : IStatusDocumentTypeService
    {
        private readonly IStatusDocumentTypeRepository _repository;
        private readonly IMapper _mapper;
        public StatusDocumentTypeService(IStatusDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IStatusDocumentTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<StatusDocumentType?> FindAsync(int statusDocumentTypeId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(statusDocumentTypeId, ct);
            return _mapper.Map<StatusDocumentType?>(data);

        }

        public StatusDocumentType? FindByDocumentAndOrder(int documentTypeId, int order)
        {
            var data = _repository.FindByDocumentAndOrder(documentTypeId, order);
            return _mapper.Map<StatusDocumentType?>(data);
        }

        public async Task<StatusDocumentType?> FindByDocumentAndOrderAsync(int documentTypeId, int order, CancellationToken ct = default)
        {
            var data = await _repository.FindByDocumentAndOrderAsync(documentTypeId, order, ct);
            return _mapper.Map<StatusDocumentType?>(data);
        }

        public async Task<IEnumerable<StatusDocumentType>> GetByDocumentTypeIdAsync(int documentTypeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByDocumentTypeIdAsync(documentTypeId, ct);
            return _mapper.Map<List<StatusDocumentType>>(data);
        }
    }
}

