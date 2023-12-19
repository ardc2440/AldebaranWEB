using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerReservationService : ICustomerReservationService
    {
        private readonly ICustomerReservationRepository _repository;
        private readonly IMapper _mapper;
        public CustomerReservationService(ICustomerReservationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(CustomerReservation customerReservation, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservation>(customerReservation) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task<IEnumerable<CustomerReservation>> GetAsync(CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(ct);
            return _mapper.Map<IEnumerable<CustomerReservation>>(data);
        }

        public async Task<IEnumerable<CustomerReservation>> GetAsync(string searchKey, CancellationToken ct = default)
        {
            var data = await _repository.GetAsync(searchKey, ct);
            return _mapper.Map<IEnumerable<CustomerReservation>>(data);
        }

        public async Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerReservationId, ct);
            return _mapper.Map<CustomerReservation?>(data);
        }

        public async Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, CancellationToken ct = default)
        {
            await _repository.CancelAsync(customerReservationId, canceledStatusDocumentId, ct);
        }

        public async Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservation>(customerReservation) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            await _repository.UpdateAsync(customerReservationId, entity, ct);
        }
    }

}
