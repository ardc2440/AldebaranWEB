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

        protected bool errorVisible;

        protected string errorMessage;

        protected CustomerReservation customerReservation;

        protected IEnumerable<Customer> customersForCUSTOMERID;

        protected IEnumerable<Employee> employeesForEMPLOYEEID;

        protected ICollection<CustomerReservationDetail> customerReservationDetails;

        protected RadzenDataGrid<CustomerReservationDetail> customerReservationDetailGrid;

        protected bool isSubmitInProgress;

        protected DocumentType documentType;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            customersForCUSTOMERID = await AldebaranDbService.GetCustomers(new Query { Expand = "City.Department.Country" });

            documentType = await AldebaranDbService.GetDocumentTypeByCode("R");

            customerReservationDetails = new List<CustomerReservationDetail>();

            customerReservation = new CustomerReservation()
            {
                CUSTOMER_RESERVATION_ID = 0,
                EMPLOYEE_ID = AldebaranDbService.GetLoggedEmployee(Security).Id,
                StatusDocumentType = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 1),
                RESERVATION_DATE = DateTime.Today,
                RESERVATION_NUMBER = "0"
            };
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

                customerReservation.RESERVATION_NUMBER = await AldebaranDbService.GenerateDocumentNumber(documentType);
                await AldebaranDbService.UpdateCustomerReservation(customerReservation);

                await DialogService.Alert($"Reserva de Articulos Guardada Satisfactoriamente con el consecutivo {customerReservation.RESERVATION_NUMBER}", "Información");
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