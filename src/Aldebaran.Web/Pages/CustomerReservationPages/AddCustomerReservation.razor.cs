using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservation
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

        protected IEnumerable<Employee> employeesForEMPLOYEEID;

        protected ICollection<CustomerReservationDetail> customerReservationDetails;

        protected RadzenDataGrid<CustomerReservationDetail> customerReservationDetailGrid;

        protected bool isSubmitInProgress;

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public string pAdjustmentId { get; set; } = "NoParamInput";

        protected override async Task OnInitializedAsync()
        {
            customersForCUSTOMERID = await AldebaranDbService.GetCustomers();

            Now = DateTime.UtcNow.AddDays(-1);

            customerReservationDetails = new List<CustomerReservationDetail>();

            var adjustmentId = 0;

            int.TryParse(pAdjustmentId, out adjustmentId);

            customerReservation = new CustomerReservation();

            customerReservation.EMPLOYEE_ID = 1;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!customerReservationDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerReservation.CustomerReservationDetails = customerReservationDetails;
                await AldebaranDbService.CreateCustomerReservation(customerReservation);
                await DialogService.Alert("Reserva de Articulos Guardada Satisfactoriamente", "Información");
                NavigationManager.NavigateTo("customer-reservations");
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
            if (await DialogService.Confirm("Está seguro que cancelar la creacion de la Reserva??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task AddCustomerReservationDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerReservationDetail>("Nueva referencia", new Dictionary<string, object> { { "customerReservationDetails", customerReservationDetails } });

            if (result == null)
                return;

            var detail = (CustomerReservationDetail)result;

            customerReservationDetails.Add(detail);

            await customerReservationDetailGrid.Reload();
        }

        protected async Task DeleteCustomerReservationDetailButtonClick(MouseEventArgs args, CustomerReservationDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                customerReservationDetails.Remove(item);

                await customerReservationDetailGrid.Reload();
            }
        }

        protected async Task EditRow(CustomerReservationDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerReservationDetail>("Actualizar referencia", new Dictionary<string, object> { { "customerReservationDetail", args } });
            if (result == null)
                return;
            var detail = (CustomerReservationDetail)result;

            customerReservationDetails.Remove(args);
            customerReservationDetails.Add(detail);

            await customerReservationDetailGrid.Reload();
        }
    }
}