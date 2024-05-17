using Aldebaran.Application.Services.Models;
using Entities = Aldebaran.DataAccess.Entities;
using Enums = Aldebaran.DataAccess.Enums;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

namespace Aldebaran.Application.Services
{
    public class CustomerOrderNotificationService : ICustomerOrderNotificationService
    {
        private readonly ICustomerOrderNotificationRepository _repository;
        private readonly IMapper _mapper;

        public CustomerOrderNotificationService(ICustomerOrderNotificationRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(ICustomerOrderNotificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async  Task AddAsync(CustomerOrderNotification customerOrderNotification, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.CustomerOrderNotification>(customerOrderNotification) ?? throw new ArgumentNullException("Notificacion no puede ser nulo.");
            await _repository.AddAsync(entity, ct);
        }

        public async  Task<IEnumerable<CustomerOrderNotification>> GetByCustomerOrderIdAsync(int customerOrderId, CancellationToken ct = default)
        {
            var data = await _repository.GetByCustomerOrderIdAsync(customerOrderId,ct);
            return _mapper.Map<List<CustomerOrderNotification>>(data);
        }

        public async Task UpdateAsync(string notificationId, NotificationStatus status, string errorMessage, CancellationToken ct = default)
        {            
            await _repository.UpdateAsync(notificationId, (Enums.NotificationStatus)status, errorMessage, ct);
        }
    }
}
