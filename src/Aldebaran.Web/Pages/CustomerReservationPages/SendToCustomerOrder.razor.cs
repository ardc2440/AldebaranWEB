using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class SendToCustomerOrder
    {
        #region Injections
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public string CustomerReservationId { get; set; } = "NoParamInput";

        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables

        protected DateTime Now { get; set; }
        protected CustomerReservation customerReservation;
        protected ICollection<CustomerReservationDetail> customerReservationDetails;
        protected LocalizedDataGrid<CustomerReservationDetail> customerReservationDetailGrid;
        protected string title;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected bool readOnly = false;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Now = DateTime.UtcNow.AddDays(-1);

                customerReservationDetails = new List<CustomerReservationDetail>();

                var customerReservationId = 0;

                int.TryParse(CustomerReservationId, out customerReservationId);

                customerReservation = await CustomerReservationService.FindAsync(customerReservationId);

                title = $"Convertir la reserva No. {customerReservation.ReservationNumber} en pedido";

                await SetPresentation();

                await GetChildData(customerReservation);
            }
            finally
            {
                isLoadingInProgress = false;
            }


        }
        #endregion

        #region Events

        async Task SetPresentation(CancellationToken ct = default)
        {
            if (Action == "view")
            {
                readOnly = true;
                title = $"Consultar la reserva No. {customerReservation.ReservationNumber}";

                return;
            }
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;

                if (!customerReservationDetails.Any(d => d.SendToCustomerOrder))
                    throw new Exception("No ha seleccionado ningúna referencia para el pedido");

                foreach (var item in customerReservationDetails.Where(d => d.SendToCustomerOrder).ToList())
                    await CustomerReservationDetailService.UpdateAsync(item.CustomerReservationDetailId, item);

                NavigationManager.NavigateTo("add-customer-order-from-reservation/" + customerReservation.CustomerReservationId);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar el envío a pedido de la reserva??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task CloseButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("");
        }

        protected async Task GetChildData(CustomerReservation args)
        {
            var customerReservationDetailsResult = await CustomerReservationDetailService.GetByCustomerReservationIdAsync(args.CustomerReservationId);
            if (customerReservationDetailsResult != null)
            {
                customerReservationDetails = customerReservationDetailsResult.ToList();
            }
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });

        #endregion
    }
}