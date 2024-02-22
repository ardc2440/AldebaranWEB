using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Notificator;
using Aldebaran.Application.Services.Notificator.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

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
        protected IPdfService PdfService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected INotificationService NotificationService { get; set; }

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
        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            CustomerReservation = await CustomerReservationService.FindAsync(Id);
            CustomerReservationDetails = await CustomerReservationDetailService.GetByCustomerReservationIdAsync(Id);
        }
        #endregion

        #region Events
        #region Print
        async Task Download(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-summary");
            var pdfBytes = await PdfService.GetBytes(html);
            await JSRuntime.InvokeVoidAsync("downloadFile", "Reserva.pdf", "application/pdf", Convert.ToBase64String(pdfBytes));
            IsBusy = false;
        }
        #endregion
        #region Notify
        async Task Notify(MouseEventArgs args)
        {
            IsBusy = true;
            var html = await JSRuntime.InvokeAsync<string>("getContent", "customer-reservation-summary");
            var pdfBytes = await PdfService.GetBytes(html);
            string pdfBase64 = Convert.ToBase64String(pdfBytes);
            string[] emails = { CustomerReservation.Customer.Email1, CustomerReservation.Customer.Email2, CustomerReservation.Customer.Email3 };

            var message = new MessageModel
            {
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