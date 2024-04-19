using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class AlarmService : IAlarmService
    {
        private readonly IAlarmRepository _repository;
        private readonly ICustomerOrderService _customerOrderService;
        private readonly ICustomerReservationService _customerReservationService;
        private readonly IPurchaseOrderService _purchaseOrderService;

        private readonly IMapper _mapper;
        public AlarmService(IAlarmRepository repository,
                            ICustomerOrderService customerOrderService,
                            ICustomerReservationService customerReservationService,
                            IPurchaseOrderService purchaseOrderService,
                            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IAlarmRepository));
            _customerOrderService = customerOrderService ?? throw new ArgumentNullException(nameof(ICustomerOrderService));
            _customerReservationService = customerReservationService ?? throw new ArgumentNullException(nameof(ICustomerReservationService));
            _purchaseOrderService = purchaseOrderService ?? throw new ArgumentNullException(nameof(IPurchaseOrderService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
        }

        public async Task<IEnumerable<Alarm>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct = default)
        {
            var data = await _repository.GetByEmployeeIdAsync(employeeId, ct);
            return _mapper.Map<List<Alarm>>(data);
        }

        public async Task<IEnumerable<Alarm>> GetByDocumentIdAsync(int documentTypeId, int documentId, CancellationToken ct = default)
        {
            var data = await _repository.GetByDocumentIdAsync(documentTypeId, documentId, ct);
            return _mapper.Map<List<Alarm>>(data);
        }

        public string GetDocumentNumber(int documentId, string documentTypeCode, CancellationToken ct = default)
        {
            switch (documentTypeCode)
            {
                case "O":
                    return GetPurchaseOrderNumber(documentId);
                case "P":
                    return GetCustomerOrderNumber(documentId);
                case "R":
                    return GetCustomerReservationNumber(documentId);
                default:
                    return "N/A";
            }
        }

        internal string GetCustomerOrderNumber(int documentId)
        {
            var customerOrderNumber = "N/A";
            var customerOrder = _customerOrderService.Find(documentId);
            if (customerOrder != null)
                customerOrderNumber = customerOrder.OrderNumber;

            return customerOrderNumber;
        }

        internal string GetCustomerReservationNumber(int documentId)
        {
            var customerReservationNumber = "N/A";
            var customerReservation = _customerReservationService.Find(documentId);
            if (customerReservation != null)
                customerReservationNumber = customerReservation.ReservationNumber;

            return customerReservationNumber;
        }

        internal string GetPurchaseOrderNumber(int documentId)
        {
            var purchaseOrderNumber = "N/A";
            var purchaseOrder = _purchaseOrderService.Find(documentId);
            if (purchaseOrder != null)
                purchaseOrderNumber = purchaseOrder.OrderNumber;

            return purchaseOrderNumber;
        }

        public async Task<Alarm> AddAsync(Alarm alarm, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.Alarm>(alarm) ?? throw new ArgumentNullException("Alarma no puede ser nula.");
            var result = await _repository.AddAsync(entity, ct);
            return _mapper.Map<Alarm>(result);
        }

        public async Task DisableAsync(int alarmId, CancellationToken ct = default)
        {
            await _repository.DisableAsync(alarmId, ct);
        }
    }

}
