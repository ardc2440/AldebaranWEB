using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CancellationRequestService: ICancellationRequestService
    {
        private readonly ICancellationRequestRepository _repository;
        private readonly IMapper _mapper;

        public CancellationRequestService(ICancellationRequestRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICancellationRequestRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(CancellationRequest cancellationRequest, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            var mapCancellationRequest = _mapper.Map<Entities.CancellationRequest>(cancellationRequest);
            await _repository.AddAsync(mapCancellationRequest, mapReason, ct);
        }

        public async Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default) => await _repository.ExistsAnyPendingRequestAsync(documentTypeId, documentNumber, ct);

        public async Task<CancellationRequest?> FindAsync(int requestId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(requestId, ct);
            return _mapper.Map<CancellationRequest?>(data);
        }

        public async Task<(IEnumerable<CancellationRequestModel>, int)> GetAllByStatusOrderAsync(int skip, int top, string search, bool getPendingRequest, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetAllByStatusOrderAsync(skip, top, search, getPendingRequest, ct);
            return (_mapper.Map<IEnumerable<CancellationRequestModel>>(data), c);
        }

        public async Task<CancellationRequest> UpdateAsync(CancellationRequest cancellationRequest, CancellationToken ct = default)
        {
            var mapCancellationRequest = _mapper.Map<Entities.CancellationRequest>(cancellationRequest);
            return _mapper.Map<CancellationRequest>(await _repository.UpdateAsync(mapCancellationRequest, ct));
        }
    }
}
