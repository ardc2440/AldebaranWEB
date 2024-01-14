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
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        [Inject]
        protected ICustomerOrderService CustomerOrderService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CUSTOMER_RESERVATION_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables
        protected IEnumerable<CustomerReservation> customerReservations;
        protected LocalizedDataGrid<CustomerReservation> CustomerReservationGrid;
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

                await GetCustomerReservationAsync();
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events

        async Task GetCustomerReservationAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            customerReservations = string.IsNullOrEmpty(searchKey) ? await CustomerReservationService.GetAsync(ct) : await CustomerReservationService.GetAsync(searchKey, ct);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (CUSTOMER_RESERVATION_ID == null)
                return;

            var valid = int.TryParse(CUSTOMER_RESERVATION_ID, out var customerReservationId);
            if (!valid)
                return;

            var customerReservation = await CustomerReservationService.FindAsync(customerReservationId, ct);
            if (customerReservation == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Reserva de artículos",
                    Severity = NotificationSeverity.Success,
                    Detail = $"La reserva No. {customerReservation.ReservationNumber} ha sido actualizada correctamente."
                });
                return;
            }

            if (Action == "create-order")
            {
                var customerOrder = await CustomerOrderService.FindAsync(customerReservation.CustomerOrderId ?? 0);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos a partir de la reserva",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El pedido de la reserva No. {customerReservation.ReservationNumber} ha sido creado correctamente, con el consecutivo {customerOrder.OrderNumber} ha sido creada correctamente."
                });
                return;
            }

            if (Action == "cancel-order")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos a partir de la reserva",
                    Severity = NotificationSeverity.Success,
                    Detail = $"la creación del pedido para la reserva No. {customerReservation.ReservationNumber} ha sido cancelada correctamente. Sus detalles continuan disponibles para un nuevo proceso."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Reserva de artículos",
                Severity = NotificationSeverity.Success,
                Detail = $"La reserva ha sido creado correctamente, con el consecutivo {customerReservation.ReservationNumber}."
            });
        }

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await CustomerReservationGrid.GoToPage(0);

            await GetCustomerReservationAsync(search);
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

                if (await DialogService.Confirm("Está seguro que desea cancelar esta reserva?") == true)
                {
                    var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 3);

                    await CustomerReservationService.CancelAsync(customerReservation.CustomerReservationId, cancelStatusDocumentType.StatusDocumentTypeId);

                    customerReservation.StatusDocumentType = cancelStatusDocumentType;
                    customerReservation.StatusDocumentTypeId = cancelStatusDocumentType.StatusDocumentTypeId;

                    await GetCustomerReservationAsync(search);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Reserva de artículos",
                        Severity = NotificationSeverity.Success,
                        Detail = $"La reserva No. {customerReservation.ReservationNumber}, ha sido cancelada correctamente."
                    });

                    await CustomerReservationGrid.Reload();
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