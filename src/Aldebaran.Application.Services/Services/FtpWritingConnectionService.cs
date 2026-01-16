using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Entities = Aldebaran.DataAccess.Entities;
using AutoMapper;

namespace Aldebaran.Application.Services.Services
{
    public class FtpWritingConnectionService : IFtpWritingConnectionService
    {
        private readonly IFtpWritingConnectionRepository _repository;
        private readonly IMapper _mapper;

        public FtpWritingConnectionService(IFtpWritingConnectionRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IFtpWritingConnectionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<FtpWritingConnection> CreateAsync(FtpWritingConnection model, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.FtpWritingConnection>(model) ?? throw new ArgumentNullException("conexión no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<FtpWritingConnection>(result);
        }

        public async Task<FtpWritingConnection> ChangeActivationAsync(int id, bool active, CancellationToken ct = default)
        {
            await _repository.ChangeActivationAsync(id, active, ct);            
            return await GetByIdAsync(id, ct);
        }

        public async Task<FtpWritingConnection> GetByIdAsync(int Id, CancellationToken ct = default)
        {
            var result = await _repository.GetByIdAsync(Id, ct);
            return _mapper.Map<FtpWritingConnection>(result);
        }

        public async Task<IEnumerable<FtpWritingConnection>> GetAllAsync(CancellationToken ct = default)
        {
            return _mapper.Map<IEnumerable<FtpWritingConnection>>(await _repository.GetAllAsync(ct));
        }

        public async Task<FtpWritingConnection> UpdateAsync(FtpWritingConnection item, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.FtpWritingConnection>(item) ?? throw new ArgumentNullException("conexión no puede ser nula.");
            var result = await _repository.UpdateAsync(entity, ct);
            return _mapper.Map<FtpWritingConnection>(result);
        }

    }
}