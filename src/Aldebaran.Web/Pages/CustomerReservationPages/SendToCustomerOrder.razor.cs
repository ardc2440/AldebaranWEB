using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

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
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public string CustomerReservationId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables
        protected DateTime Now { get; set; }

        protected bool errorVisible;

        protected string errorMessage;

        protected CustomerReservation customerReservation;

        protected ICollection<CustomerReservationDetail> customerReservationDetails;

        protected RadzenDataGrid<CustomerReservationDetail> customerReservationDetailGrid;

        protected bool isSubmitInProgress;
        protected string title;

        #endregion
        #region Overrides
        protected override async Task OnInitializedAsync()
        {

            Now = DateTime.UtcNow.AddDays(-1);

            customerReservationDetails = new List<CustomerReservationDetail>();

            var customerReservationId = 0;

            int.TryParse(CustomerReservationId, out customerReservationId);

            customerReservation = await CustomerReservationService.FindAsync(customerReservationId);

            title = $"Convertir la Reserva No. {customerReservation.ReservationNumber} en pedido";

            await GetChildData(customerReservation);

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!customerReservationDetails.Any(d => d.SendToCustomerOrder))
                    throw new Exception("No ha seleccionado ningun artículo para el pedido");

                foreach (var item in customerReservationDetails.Where(d => d.SendToCustomerOrder).ToList())
                    await CustomerReservationDetailService.UpdateAsync(item.CustomerReservationDetailId, item);

                NavigationManager.NavigateTo("add-customer-order-from-reservation/" + customerReservation.CustomerReservationId);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que cancelar el envio a Pedido de la Reserva??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task GetChildData(CustomerReservation args)
        {
            var customerReservationDetailsResult = await CustomerReservationDetailService.GetAsync(args.CustomerReservationId);
            if (customerReservationDetailsResult != null)
            {
                customerReservationDetails = customerReservationDetailsResult.ToList();
            }
        }
        #endregion
    }
}