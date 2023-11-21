using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class SendToCustomerOrder
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        protected DateTime Now { get; set; }

        protected bool errorVisible;

        protected string errorMessage;

        protected CustomerReservation customerReservation;

        protected IEnumerable<Customer> customersForCUSTOMERID;

        protected ICollection<CustomerReservationDetail> customerReservationDetails;

        protected RadzenDataGrid<CustomerReservationDetail> customerReservationDetailGrid;

        protected bool isSubmitInProgress;
        protected string title;

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public string pCustomerReservationId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            customersForCUSTOMERID = await AldebaranDbService.GetCustomers();

            Now = DateTime.UtcNow.AddDays(-1);

            customerReservationDetails = new List<CustomerReservationDetail>();

            var customerReservationId = 0;

            int.TryParse(pCustomerReservationId, out customerReservationId);

            customerReservation = AldebaranDbService.GetCustomerReservationByCustomerReservationId(customerReservationId).Result;

            title = $"Convertir la Reserva No. {customerReservation.RESERVATION_NUMBER} en pedido";

            await GetChildData(customerReservation);

        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!customerReservationDetails.Any(d => d.SEND_TO_CUSTOMER_ORDER))
                    throw new Exception("No ha seleccionado ningun artículo para el pedido");

                await AldebaranDbService.UpdateCustomerReservationDetails(customerReservationDetails.Where(d => d.SEND_TO_CUSTOMER_ORDER).ToList());

                NavigationManager.NavigateTo("add-customer-order-from-reservation/" + customerReservation.CUSTOMER_RESERVATION_ID);
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
            var customerReservationDetailsResult = await AldebaranDbService.GetCustomerReservationDetails(new Query { Filter = $@"i => i.CUSTOMER_RESERVATION_ID == {args.CUSTOMER_RESERVATION_ID}", Expand = "CustomerReservation, ItemReference, ItemReference.Item" });
            if (customerReservationDetailsResult != null)
            {
                customerReservationDetails = customerReservationDetailsResult.ToList();
            }
        }
    }
}