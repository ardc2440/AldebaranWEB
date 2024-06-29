using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.DataAccess.Infraestructure.Repository;
using Aldebaran.Infraestructure.Common.Utils;
using AutoMapper;
using Entities = Aldebaran.DataAccess.Entities;

namespace Aldebaran.Application.Services
{
    public class PurchaseOrderNotificationService : IPurchaseOrderNotificationService
    {
        private readonly IPurchaseOrderNotificationRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ISharedStringLocalizer _sharedLocalizer;
        
        public PurchaseOrderNotificationService(IPurchaseOrderNotificationRepository repository, IMapper mapper, INotificationService notificationService,
            ISharedStringLocalizer sharedLocalizer)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(IPurchaseOrderNotificationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(IMapper));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(INotificationService));
            _sharedLocalizer = sharedLocalizer ?? throw new ArgumentNullException(nameof(ISharedStringLocalizer));
        }

        public async Task<PurchaseOrderNotification?> FindAsync(int purchaseOrderNotificationId, CancellationToken ct = default)
        {
            var entity = await _repository.FindAsync(purchaseOrderNotificationId, ct);
            return _mapper.Map<PurchaseOrderNotification>(entity);
        }

        public async Task AddAsync(PurchaseOrderNotification purchaseOrderNotification, CancellationToken ct = default)
        {
            var entity = _mapper.Map<Entities.PurchaseOrderNotification>(purchaseOrderNotification) ?? throw new ArgumentNullException("Notificación de la orden de compra no puede ser nula.");
            await _repository.AddAsync(entity, ct);
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByModifiedPurchaseOrder(int modifiedPurchaseOrderId, CancellationToken ct = default)
        {
            var entity = await _repository.GetByModifiedPurchaseOrder(modifiedPurchaseOrderId, ct);
            return _mapper.Map<List<PurchaseOrderNotification>>(entity);
        }

        public async Task<IEnumerable<PurchaseOrderNotification>> GetByPurchaseOrderId(int purchaseOrderId, CancellationToken ct = default)
        {
            var entity = await _repository.GetByPurchaseOrderId(purchaseOrderId, ct);
            return _mapper.Map<List<PurchaseOrderNotification>>(entity);
        }
        public async Task UpdateAsync(int purchaseOrderNotificationId, string uid, NotificationStatus status, CancellationToken ct = default)
        {
            await _repository.UpdateAsync(purchaseOrderNotificationId, uid, (DataAccess.Enums.NotificationStatus)status, ct);
        }
        public async Task UpdateNotificationResponseAsync(string notificationId, NotificationStatus status, string errorMessage, DateTime date, CancellationToken ct = default)
        {
            await _repository.UpdateNotificationResponseAsync(notificationId, (DataAccess.Enums.NotificationStatus)status, errorMessage, date, ct);
        }

        public async Task NotifyToCustomers(int modifiedPurchaseOrderId, string baseUri, CancellationToken ct = default)
        {
            var NotificationTemplateName = "PurchaseOrder:Update:Customer:Order";
            var purchaseOrderNotifications = await GetByModifiedPurchaseOrder(modifiedPurchaseOrderId, ct);
            foreach (var pon in purchaseOrderNotifications)
            {
                string[] emails = pon.NotifiedMailList.Split(";");
                var uid = Guid.NewGuid().ToString();
                var message = new MessageModel
                {
                    HookUrl = new Uri($"{baseUri.TrimEnd('/')}/Notification/PurchaseOrderUpdate"),
                    Header = new MessageModel.EnvelopeHeader
                    {
                        MessageUid = uid,
                        ReceiverUrn = emails.Where(s => s != null).ToArray(),
                        Subject = "Sales",
                    },
                    Body = new MessageModel.EnvelopeBody
                    {
                        Template = NotificationTemplateName,
                    }
                };
                var additionalBodyMessage = $"<p>Datos del pedido afectado: <br /><br />" +
                                            $"Pedido No.: {pon.CustomerOrder.OrderNumber}<br />" +
                                            $"Fecha de pedido: {pon.CustomerOrder.OrderDate.ToString(_sharedLocalizer["date:format"])}<br />" +
                                            $"Fecha estimada de entrega: {pon.CustomerOrder.EstimatedDeliveryDate.ToString(_sharedLocalizer["date:format"])}</p>";
                await UpdateAsync(pon.PurchaseOrderNotificationId, uid, NotificationStatus.InProcess, ct);
                await _notificationService.Send(message, additionalBodyMessage, ct);
            }
        }
    }
}
