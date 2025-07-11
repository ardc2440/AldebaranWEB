﻿using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services.Services
{
    public class PackagingService : IPackagingService
    {
        private readonly IPackagingRepository _repository;
        private readonly IMapper _mapper;

        public PackagingService(IPackagingRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPackagingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<Packaging?> FindByItemId(int itemId, CancellationToken ct = default)
        {
            var data = await _repository.FindByItemId(itemId, ct);
            return _mapper.Map<Packaging?>(data);
        }
    }
}
