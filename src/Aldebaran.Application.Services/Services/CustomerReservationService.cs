﻿using Aldebaran.Application.Services.Models;
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

        public async Task<CustomerReservation> AddAsync(CustomerReservation customerReservation, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservation>(customerReservation) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<CustomerReservation>(result);
        }

        public async Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, ct);
            return (_mapper.Map<IEnumerable<CustomerReservation>>(data), count);
        }

        public async Task<(IEnumerable<CustomerReservation>, int)> GetAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, count) = await _repository.GetAsync(skip, top, searchKey, ct);
            
            return (_mapper.Map<IEnumerable<CustomerReservation>>(data), count);
        }

        public async Task<CustomerReservation?> FindAsync(int customerReservationId, CancellationToken ct = default)
        {
            var data = await _repository.FindAsync(customerReservationId, ct);
            return _mapper.Map<CustomerReservation?>(data);
        }

        public async Task CancelAsync(int customerReservationId, short canceledStatusDocumentId, Reason reason, CancellationToken ct = default)
        {
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason>(reason);
            await _repository.CancelAsync(customerReservationId, canceledStatusDocumentId, mapReason, ct);
        }

        public async Task UpdateAsync(int customerReservationId, CustomerReservation customerReservation, Reason? reason, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservation>(customerReservation) ?? throw new ArgumentNullException("Reserva no puede ser nula.");
            var mapReason = _mapper.Map<DataAccess.Infraestructure.Models.Reason?>(reason);
            await _repository.UpdateAsync(customerReservationId, entity, mapReason, ct);
        }

        public async Task<(IEnumerable<CustomerReservation> customerReservations, int count)> GetAsync(int skip, int take, string filter, string orderBy, CancellationToken ct = default)
        {
            var (d, r) = await _repository.GetAsync(skip, take, filter, orderBy, ct);
            var data = _mapper.Map<IEnumerable<CustomerReservation>>(d);
            return (data, r);
        }

        /* Logs */
        public async Task<(IEnumerable<ModifiedCustomerReservation>, int count)> GetCustomerReservationModificationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetCustomerReservationModificationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<ModifiedCustomerReservation>>(data), c);
        }
        public async Task<(IEnumerable<CanceledCustomerReservation>, int count)> GetCustomerReservationCancellationsLogAsync(int skip, int top, string searchKey, CancellationToken ct = default)
        {
            var (data, c) = await _repository.GetCustomerReservationCancellationsLogAsync(skip, top, searchKey, ct);
            return (_mapper.Map<IEnumerable<CanceledCustomerReservation>>(data), c);
        }
    }
}
