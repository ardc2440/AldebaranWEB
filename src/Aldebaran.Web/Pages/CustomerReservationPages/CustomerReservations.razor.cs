using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class CustomerReservations
    {
        #region Injections
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        #endregion

        #region Global Variables
        protected IEnumerable<CustomerReservation> customerReservations;
        protected LocalizedDataGrid<CustomerReservation> grid0;
        protected DialogResult DialogResult { get; set; }
        protected string search = "";
        protected bool isLoadingInProgress;
        protected DocumentType documentType;
        protected CustomerReservation customerReservation;
        protected LocalizedDataGrid<CustomerReservationDetail> CustomerReservationDetailsDataGrid;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("R");

                isLoadingInProgress = true;

                await Task.Yield();

                customerReservations = await CustomerReservationService.GetAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            customerReservations = await CustomerReservationService.GetAsync(search);

        }
        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-reservation");
        }
        protected async Task EditRow(CustomerReservation args)
        {
            NavigationManager.NavigateTo("edit-customer-reservation/" + args.CustomerReservationId);
        }

        protected async Task CancelCustomerReservation(MouseEventArgs arg, CustomerReservation customerReservation)
        {
            try
            {
                DialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar esta reserva?") == true)
                {
                    /* TO DO Agregar Ventana pára el ingreso del motivo de cancelacion de la reserva */

                    var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 3);

                    await CustomerReservationService.CancelAsync(customerReservation.CustomerReservationId, cancelStatusDocumentType.StatusDocumentTypeId);

                    customerReservation.StatusDocumentType = cancelStatusDocumentType;
                    customerReservation.StatusDocumentTypeId = cancelStatusDocumentType.StatusDocumentTypeId;

                    await DialogService.Alert($"Reserva cancelada correctamente", "Información");
                    await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar la reserva"
                });
            }
        }

        protected async Task GetChildData(CustomerReservation args)
        {
            customerReservation = args;
            var CustomerReservationDetailsResult = await CustomerReservationDetailService.GetByCustomerReservationIdAsync(args.CustomerReservationId);
            if (CustomerReservationDetailsResult != null)
            {
                args.CustomerReservationDetails = CustomerReservationDetailsResult.ToList();
            }
        }

        protected async Task<bool> CanEdit(CustomerReservation customerReservation)
        {
            return Security.IsInRole("Admin", "Customer Reservation Editor") && customerReservation.StatusDocumentType.EditMode;
        }

        protected async Task<bool> CanSendToCustomerOrder(CustomerReservation customerReservation)
        {
            return Security.IsInRole("Admin", "Customer Order Editor") && customerReservation.StatusDocumentType.EditMode;
        }

        protected async Task SendToCustomerOrder(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/" + args.CustomerReservationId);
        }
        #endregion
    }
}