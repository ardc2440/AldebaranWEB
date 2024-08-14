using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Aldebaran.Infraestructure.Common.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class CustomerOrderSummary
    {
        #region Injections
        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentService CustomerOrderShipmentService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected ICustomerOrderNotificationService CustomerOrderNotificationService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected INotificationService NotificationService { get; set; }

        [Inject]
        protected INotificationTemplateService NotificationTemplateService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string NotificationTemplateName { get; set; }
        #endregion

        #region Variables
        protected CustomerOrder CustomerOrder;
        protected IEnumerable<CustomerOrderDetail> CustomerOrderDetails;
        protected IEnumerable<CustomerOrderShipment> CustomerOrderShipments;
        private bool IsBusy = false;
        protected bool isLoadingInProgress;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                CustomerOrder = await CustomerOrderService.FindAsync(Id);
                CustomerOrderDetails = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(Id);
                CustomerOrderShipments = await CustomerOrderShipmentService.GetByCustomerOrderIdAsync(Id);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        #region Print
        async Task Save(RadzenSplitButtonItem args)
        {
            if (args?.Value == null)
                return;
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-summary");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Pedidos.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "customer-order-summary");
            }
            IsBusy = false;
        }
        #endregion
        #region Notify
        async Task Notify(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-order-summary");
            var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            string[] emails = CustomerOrder.Customer.Email.Split(";");

            var message = new MessageModel
            {
                HookUrl = new Uri($"{NavigationManager.BaseUri.TrimEnd('/')}/Notification/CustomerOrderUpdate"),
                Header = new MessageModel.EnvelopeHeader
                {
                    MessageUid = Guid.NewGuid().ToString(),
                    ReceiverUrn = emails.Where(s => s != null).ToArray(),
                    Subject = "Sales",
                },
                Body = new MessageModel.EnvelopeBody
                {
                    Template = NotificationTemplateName,
                    Medias = new List<MessageModel.EnvelopeBody.MediaContent>
                    {
                        new MessageModel.EnvelopeBody.MediaContent
                        {
                            ContentType = "application/pdf",
                            FileName="Pedidos.pdf",
                            Hash= pdfBase64
                        }
                    }
                }
            };
            
            await CustomerOrderNotificationService.AddAsync(new CustomerOrderNotification
            {
                CustomerOrderId = CustomerOrder.CustomerOrderId,
                NotificationId = message.Header.MessageUid,
                NotificationState = NotificationStatus.InProcess,
                NotificationTemplateId = (await NotificationTemplateService.FindAsync(NotificationTemplateName)).NotificationTemplateId, 
                NotifiedMailList = CustomerOrder.Customer.Email,
                NotificationDate = DateTime.Now
            });

            await NotificationService.Send(message);

            // Enviar notificacion
            DialogService.Close(true);
            IsBusy = false;
        }
        void CancelNotify(MouseEventArgs args)
        {
            DialogService.Close(false);
        }
        #endregion
        static string GetOrderStatus(CustomerOrderDetail detail, CustomerOrder master)
        {
            if (master.StatusDocumentType.StatusOrder == 6)
                return "Cancelado";
            if (detail.DeliveredQuantity == detail.RequestedQuantity && detail.ProcessedQuantity == 0)
                return "Totalmente atentido";            
            if (master.StatusDocumentType.StatusOrder == 5)
                return "Cerrado";
            if (detail.DeliveredQuantity == 0 && detail.ProcessedQuantity == 0)
                return "Pendiente";
            if (detail.ProcessedQuantity > 0)
                return "En proceso";
            if (detail.DeliveredQuantity < detail.RequestedQuantity)
                return "Parcialmente atentido";
            
            return null;
        }
        #endregion
    }
}