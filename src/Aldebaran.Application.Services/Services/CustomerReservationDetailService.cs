using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class CustomerReservationDetailService : ICustomerReservationDetailService
    {
        private readonly ICustomerReservationDetailRepository _repository;
        private readonly IMapper _mapper;
        public CustomerReservationDetailService(ICustomerReservationDetailRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationDetailRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<CustomerReservationDetail>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerReservationIdAsync(customerReservationId, ct);
            return _mapper.Map<IEnumerable<CustomerReservationDetail>>(data);
        }

        public async Task UpdateAsync(int customerReservationDetailId, CustomerReservationDetail customerReservationDetail, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservationDetail>(customerReservationDetail) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            await _repository.UpdateAsync(customerReservationDetailId, entity, ct);
        }
    }
}
