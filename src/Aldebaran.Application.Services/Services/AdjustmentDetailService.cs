﻿using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class AdjustmentDetailService : IAdjustmentDetailService
    {
        private readonly IAdjustmentDetailRepository _repository;
        private readonly IMapper _mapper;
        public AdjustmentDetailService(IAdjustmentDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAdjustmentDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetAsync(string filter, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(filter, ct);
            return _mapper.Map<List<AdjustmentDetail>>(data);
        }

        public async Task<AdjustmentDetail?> FindAsync(int adjustmentDetailId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(adjustmentDetailId, ct);
            return _mapper.Map<AdjustmentDetail?>(data);
        }

        public async Task<IEnumerable<AdjustmentDetail>> GetByAdjustmentIdAsync(int adjustmentId, CancellationToken ct = default)
        {
            var data = await _repository.GetByAdjustmentIdAsync(adjustmentId, ct);
            return _mapper.Map<List<AdjustmentDetail>>(data);
        }
    }

}
