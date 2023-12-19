using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservation
    {
        #region Injections
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;

        protected string errorMessage;

        protected CustomerReservation customerReservation;

        protected IEnumerable<Customer> customersForCUSTOMERID;

        protected IEnumerable<Employee> employeesForEMPLOYEEID;

        protected ICollection<CustomerReservationDetail> customerReservationDetails;

        protected RadzenDataGrid<CustomerReservationDetail> customerReservationDetailGrid;

        protected bool isSubmitInProgress;

        protected DocumentType documentType;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            customersForCUSTOMERID = await CustomerService.GetAsync();

            documentType = await DocumentTypeService.FindByCodeAsync("R");

            customerReservationDetails = new List<CustomerReservationDetail>();

            customerReservation = new CustomerReservation()
            {
                CustomerReservationId = 0,
                Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id),
                StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1),
                ReservationDate = DateTime.Today,
                ReservationNumber = "0"
            };
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!customerReservationDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                customerReservation.CustomerReservationDetails = customerReservationDetails;

                await CustomerReservationService.AddAsync(customerReservation);

                await DialogService.Alert($"Reserva de Articulos Guardada Satisfactoriamente con el consecutivo {customerReservation.ReservationNumber}", "Informaci�n");
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
            if (await DialogService.Confirm("Est� seguro que cancelar la creacion de la Reserva??", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task AddCustomerReservationDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddCustomerReservationDetail>("Nueva referencia", new Dictionary<string, object> { { "CustomerReservationDetails", customerReservationDetails } });

            if (result == null)
                return;

            var detail = (CustomerReservationDetail)result;

            customerReservationDetails.Add(detail);

            await customerReservationDetailGrid.Reload();
        }

        protected async Task DeleteCustomerReservationDetailButtonClick(MouseEventArgs args, CustomerReservationDetail item)
        {
            if (await DialogService.Confirm("Est� seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                customerReservationDetails.Remove(item);

                await customerReservationDetailGrid.Reload();
            }
        }

        protected async Task EditRow(CustomerReservationDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerReservationDetail>("Actualizar referencia", new Dictionary<string, object> { { "CustomerReservationDetail", args } });
            if (result == null)
                return;
            var detail = (CustomerReservationDetail)result;

            await customerReservationDetailGrid.Reload();
        }
        #endregion
    }
}