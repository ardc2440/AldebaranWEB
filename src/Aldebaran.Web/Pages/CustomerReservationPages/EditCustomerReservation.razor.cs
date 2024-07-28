using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
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
        protected TooltipService TooltipService { get; set; }

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
        protected CustomerReservation customerReservation;
        protected IEnumerable<Customer> customersForCUSTOMERID;
        protected ICollection<CustomerReservationDetail> customerReservationDetails;
        protected LocalizedDataGrid<CustomerReservationDetail> customerReservationDetailGrid;
        protected string title;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;

        protected int count = 0;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                
                Now = DateTime.UtcNow.AddDays(-1);

                customerReservationDetails = new List<CustomerReservationDetail>();

                _ = int.TryParse(CustomerReservationId, out var customerReservationId);

                customerReservation = await CustomerReservationService.FindAsync(customerReservationId);

                await LoadData(new LoadDataArgs { Filter = customerReservation.Customer.CustomerName, Skip = 0, Top = 1 });


                title = $"Actualizar la Reserva No. {customerReservation.ReservationNumber}";

                await GetChildData(customerReservation);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task LoadData(LoadDataArgs args)
        {
            await Task.Yield();
            var (customers, _count) = string.IsNullOrEmpty(args.Filter) ? await CustomerService.GetAsync(args.Skip.Value, args.Top.Value) : await CustomerService.GetAsync(args.Skip.Value, args.Top.Value, args.Filter);
            customersForCUSTOMERID = customers.ToList();
            count = _count;
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                if (!customerReservationDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                var reasonResult = await DialogService.OpenAsync<ModificationReasonDialog>("Confirmar modificación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "R" }, { "TITLE", "Está seguro que desea actualizar esta reserva?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;
                customerReservation.CustomerReservationDetails = customerReservationDetails;
                await CustomerReservationService.UpdateAsync(customerReservation.CustomerReservationId, customerReservation, reason);
                var result = await DialogService.OpenAsync<CustomerReservationSummary>(null, new Dictionary<string, object> { { "Id", customerReservation.CustomerReservationId }, { "NotificationTemplateName", "Customer:Reservation:Update" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                NavigationManager.NavigateTo($"customer-reservations/edit/{customerReservation.CustomerReservationId}");
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
            if (await DialogService.Confirm("Está seguro que desea cancelar la modificación de la Reserva?", "Confirmar") == true)
                NavigationManager.NavigateTo("customer-reservations");
        }

        protected async Task EditRow(CustomerReservationDetail args)
        {
            var result = await DialogService.OpenAsync<EditCustomerReservationDetail>("Actualizar referencia", new Dictionary<string, object> { { "CustomerReservationDetail", args } });

            if (result == null)
                return;

            args.ReservedQuantity = result.ReservedQuantity;
            args.Brand = result.Brand;

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