using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class CustomerReservations
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

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.AldebaranDb.CustomerReservation> customerReservations;

        protected RadzenDataGrid<Models.AldebaranDb.CustomerReservation> grid0;

        protected DialogResult dialogResult { get; set; }

        protected string search = "";

        protected bool isLoadingInProgress;

        protected DocumentType documentType { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            customerReservations = await AldebaranDbService.GetCustomerReservations(new Query { Filter = $@"i => (i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.RESERVATION_NUMBER.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Customer,StatusDocumentType,Employee" });

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await AldebaranDbService.GetDocumentTypeByCode("R");

                isLoadingInProgress = true;

                await Task.Yield();

                customerReservations = await AldebaranDbService.GetCustomerReservations(new Query { Filter = $@"i => (i.StatusDocumentType.DOCUMENT_TYPE_ID.Equals(@1) && i.StatusDocumentType.STATUS_DOCUMENT_TYPE_NAME.Contains(@0)) || i.Customer.CUSTOMER_NAME.Contains(@0) || i.RESERVATION_NUMBER.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search, documentType.DOCUMENT_TYPE_ID }, Expand = "Customer,StatusDocumentType,Employee" });
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-reservation");
        }

        protected async Task EditRow(Models.AldebaranDb.CustomerReservation args)
        {
            NavigationManager.NavigateTo("edit-customer-reservation/" + args.CUSTOMER_RESERVATION_ID);
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.CustomerReservation customerReservation)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea eliminar esta reserva?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteCustomerReservation(customerReservation.CUSTOMER_RESERVATION_ID);

                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Reserva eliminada correctamente." };
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el ajuste"
                });
            }
        }

        protected Models.AldebaranDb.CustomerReservation customerReservation;

        protected async Task GetChildData(Models.AldebaranDb.CustomerReservation args)
        {
            customerReservation = args;
            var CustomerReservationDetailsResult = await AldebaranDbService.GetCustomerReservationDetails(new Query { Filter = $@"i => i.CUSTOMER_RESERVATION_ID == {args.CUSTOMER_RESERVATION_ID}", Expand = "CustomerReservation,ItemReference,ItemReference.Item" });
            if (CustomerReservationDetailsResult != null)
            {
                args.CustomerReservationDetails = CustomerReservationDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.CustomerReservationDetail> CustomerReservationDetailsDataGrid;

        protected async Task<bool> CanEdit(CustomerReservation customerReservation)
        {
            return Security.IsInRole("Admin", "Customer Reservation Editor") && customerReservation.StatusDocumentType.EDIT_MODE;
        }

        protected async Task SendToCustomerOrder(Models.AldebaranDb.CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/" + args.CUSTOMER_RESERVATION_ID);
        }
    }
}