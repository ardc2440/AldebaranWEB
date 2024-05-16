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

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class CustomerReservationSummary
    {
        #region Injections
        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        [Inject]
        protected IFileBytesGeneratorService FileBytesGeneratorService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected INotificationService NotificationService { get; set; }

        [Inject]
        protected INotificationTemplateService NotificationTemplateService { get; set; }

        [Inject]
        protected ICustomerReservationNotificationService CustomerReservationNotificationService { get; set; }

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
        protected CustomerReservation CustomerReservation;
        protected IEnumerable<CustomerReservationDetail> CustomerReservationDetails;
        private bool IsBusy = false;
        protected bool isLoadingInProgress;
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                CustomerReservation = await CustomerReservationService.FindAsync(Id);
                CustomerReservationDetails = await CustomerReservationDetailService.GetByCustomerReservationIdAsync(Id);
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
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-summary");
            if (args?.Value == "save")
            {
                var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html, true);
                await JSRuntime.InvokeVoidAsync("downloadFile", "Reserva.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            }
            if (args?.Value == "print")
            {
                await JSRuntime.InvokeVoidAsync("print", "customer-reservation-summary");
            }
            IsBusy = false;
        }
        #endregion
        #region Notify
        async Task Notify(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-summary");
            var pdfBytes = await FileBytesGeneratorService.GetPdfBytes(html);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            string[] emails = CustomerReservation.Customer.Email.Split(";");

            var message = new MessageModel
            {
                HookUrl = new Uri($"{NavigationManager.BaseUri}Application/CustomerReservationUpdateAsync"),
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
                            FileName="Reserva.pdf",
                            Hash= pdfBase64
                        }
                    }
                }
            };
            await NotificationService.Send(message);

            await CustomerReservationNotificationService.AddAsync(new CustomerReservationNotification
            {
                CustomerReservationId = CustomerReservation.CustomerReservationId,
                NotificationId = message.Header.MessageUid,
                NotificationState = NotificationStatus.InProcess,
                NotificationTemplateId = (await NotificationTemplateService.FindAsync(NotificationTemplateName)).NotificationTemplateId,
                NotifiedMailList = CustomerReservation.Customer.Email,
                NotificationDate = DateTime.Now
            });

            // Enviar notificacion
            DialogService.Close(true);
            IsBusy = false;
        }
        void CancelNotify(MouseEventArgs args)
        {
            DialogService.Close(false);
        }
        #endregion
        #endregion
    }
}