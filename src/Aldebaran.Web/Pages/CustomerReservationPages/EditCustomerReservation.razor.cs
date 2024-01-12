using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class EditCustomerReservation
    {
        #region Injections
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

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
        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected ICollection<CustomerReservationDetail> customerReservationDetails;
        protected LocalizedDataGrid<CustomerReservationDetail> customerReservationDetailGrid;
        protected bool isSubmitInProgress;
        protected string title;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            customersForCUSTOMERID = await CustomerService.GetAsync();

            Now = DateTime.UtcNow.AddDays(-1);

            customerReservationDetails = new List<CustomerReservationDetail>();

            _ = int.TryParse(CustomerReservationId, out var customerReservationId);

            customerReservation = await CustomerReservationService.FindAsync(customerReservationId);

            title = $"Modificar la Reserva No. {customerReservation.ReservationNumber}";

            await GetChildData(customerReservation);
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

                await CustomerReservationService.UpdateAsync(customerReservation.CustomerReservationId, customerReservation);

                await DialogService.Alert($"Reserva {customerReservation.ReservationNumber} modificada satisfactoriamente", "Información");
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
            if (await DialogService.Confirm("Está seguro que cancelar la modificación de la Reserva??", "Confirmar") == true)
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
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
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

            customerReservationDetails.Remove(args);
            customerReservationDetails.Add(detail);

            await customerReservationDetailGrid.Reload();
        }

        protected async Task GetChildData(CustomerReservation args)
        {
            var customerReservationDetailsResult = await CustomerReservationDetailService.GetByCustomerReservationIdAsync(args.CustomerReservationId);
            if (customerReservationDetailsResult != null)
            {
                customerReservationDetails = customerReservationDetailsResult.ToList();
            }
        }
        #endregion
    }
}