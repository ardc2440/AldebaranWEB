using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldebaran.Application.Services.Services
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

        public Task<bool> ExistsAnyPendingRequestAsync(short documentTypeId, int documentNumber, CancellationToken ct = default) => _repository.ExistsAnyPendingRequestAsync(documentTypeId, documentNumber, ct);
    }
}
