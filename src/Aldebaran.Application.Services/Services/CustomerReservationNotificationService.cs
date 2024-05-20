using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Enums = Aldebaran.DataAccess.Enums;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerReservationNotificationService : ICustomerReservationNotificationService
    {
        private readonly ICustomerReservationNotificationRepository _repository;
        private readonly IMapper _mapper;

        public CustomerReservationNotificationService(ICustomerReservationNotificationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerReservationNotificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task AddAsync(CustomerReservationNotification customerReservationNotification, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerReservationNotification>(customerReservationNotification) ?? throw new ArgumentNullException("Notificacion no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task<IEnumerable<CustomerReservationNotification>> GetByCustomerReservationIdAsync(int customerReservationId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerReservationIdAsync(customerReservationId, ct);
            return _mapper.Map<List<CustomerReservationNotification>>(data);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            await _repository.UpdateAsync(notificationId, (Enums.NotificationStatus)status, errorMessage, date, ct);
        }
    }
}
