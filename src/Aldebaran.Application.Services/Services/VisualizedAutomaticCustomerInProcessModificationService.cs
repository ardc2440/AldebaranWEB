using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class VisualizedAutomaticCustomerInProcessModificationService : IVisualizedAutomaticCustomerInProcessModificationService
    {
        private readonly IVisualizedAutomaticCustomerInProcessModificationRepository _repository;
        private readonly IMapper _mapper;
        public VisualizedAutomaticCustomerInProcessModificationService(IVisualizedAutomaticCustomerInProcessModificationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IVisualizedAutomaticCustomerInProcessModificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }
        public async Task AddAsync(VisualizedAutomaticCustomerOrderInProcessModification item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.VisualizedAutomaticCustomerOrderInProcessModification>(item) ?? throw new ArgumentNullException("El elemento no puede ser nulo    .");
            await _repository.AddAsync(entity, ct);            
        }
    }
}
