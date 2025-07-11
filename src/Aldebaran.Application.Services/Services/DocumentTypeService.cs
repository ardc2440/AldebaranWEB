﻿using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class DocumentTypeService : IDocumentTypeService
    {
        private readonly IDocumentTypeRepository _repository;
        private readonly IMapper _mapper;
        public DocumentTypeService(IDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IDocumentTypeRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<DocumentType?> FindByCodeAsync(string code, CancellationToken ct = default)
        {
            var data = await _repository.FindByCodeAsync(code, ct);
            return _mapper.Map<DocumentType?>(data);
        }
    }
}
