using Aldebaran.Application.Services.Models;
using Aldebaran.DataAccess.Infraestructure.Repository;
using AutoMapper;

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

        public async Task<string> GetDocumentNumber(int documentId, string documentTypeCode, CancellationToken ct = default)
        {
            switch (documentTypeCode)
            {
                case "O":
                    return await GetPurchaseOrderNumber(documentId);
                case "P":
                    return await GetCustomerOrderNumber(documentId);
                case "R":
                    return await GetCustomerReservationNumber(documentId);
                default:
                    return "N/A";
            }
        }

        internal async Task<string> GetCustomerOrderNumber(int documentId, CancellationToken ct = default)
        {
            var customerOrderNumber = "N/A"; 
            var customerOrder = await _customerOrderService.FindAsync(documentId, ct);
            if (customerOrder !=null)
                customerOrderNumber = customerOrder.OrderNumber;

            return customerOrderNumber;
        }

        internal async Task<string> GetCustomerReservationNumber(int documentId, CancellationToken ct = default)
        {
            var customerReservationNumber = "N/A";
            var customerReservation = await _customerReservationService.FindAsync(documentId, ct);
            if (customerReservation != null)
                customerReservationNumber = customerReservation.ReservationNumber;

            return customerReservationNumber;
        }

        internal async Task<string> GetPurchaseOrderNumber(int documentId, CancellationToken ct = default)
        {
            var purchaseOrderNumber = "N/A";
            var purchaseOrder = await _purchaseOrderService.FindAsync(documentId, ct);
            if (purchaseOrder != null)
                purchaseOrderNumber = purchaseOrder.OrderNumber;

            return purchaseOrderNumber;
        }

    }

}
