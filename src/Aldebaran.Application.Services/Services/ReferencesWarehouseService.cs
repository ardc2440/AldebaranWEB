﻿using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class ReferencesWarehouseService : IReferencesWarehouseService
    {
        private readonly IReferencesWarehouseRepository _repository;
        private readonly IMapper _mapper;
        public ReferencesWarehouseService(IReferencesWarehouseRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IReferencesWarehouseRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<ReferencesWarehouse?> GetByReferenceAndWarehouseIdAsync(int referenceId, short warehouseId, CancellationToken ct = default)
        {
            var data = await _repository.GetByReferenceAndWarehouseIdAsync(referenceId, warehouseId, ct);
            return _mapper.Map<ReferencesWarehouse>(data);
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetByReferenceIdAsync(int referenceId, CancellationToken ct = default)
        {
            var data = await _repository.GetByReferenceIdAsync(referenceId, ct);
            return _mapper.Map<IEnumerable<ReferencesWarehouse>>(data);
        }

        public async Task<IEnumerable<ReferencesWarehouse>> GetAllAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAllAsync(ct);
            return _mapper.Map<IEnumerable<ReferencesWarehouse>>(data);
        }

    }

}
