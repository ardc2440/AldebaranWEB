using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Identity.Client;
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

        [Inject]
        protected IAlarmService AlarmService { get; set; }

        [Inject]
        protected ICustomerReservationNotificationService CustomerReservationNotificationService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CUSTOMER_RESERVATION_ID { get; set; } = null;

        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables
        protected DialogResult dialogResult;
        protected IEnumerable<CustomerReservation> customerReservations;
        protected LocalizedDataGrid<CustomerReservation> CustomerReservationGrid;
        protected DialogResult DialogResult { get; set; }
        protected string search = "";
        protected bool isLoadingInProgress;
        protected DocumentType documentType;
        protected CustomerReservation customerReservation;
        protected LocalizedDataGrid<CustomerReservationDetail> CustomerReservationDetailsDataGrid;
        protected LocalizedDataGrid<CustomerReservationNotification> CustomerReservationNotificationsDataGrid;

        protected IEnumerable<Application.Services.Models.Alarm> alarms;
        protected LocalizedDataGrid<Application.Services.Models.Alarm> alarmsGrid;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("R");

                isLoadingInProgress = true;

                await Task.Yield();
                                
                await DialogResultResolver();
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
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetCustomerReservationAsync(search);
        }

        async Task GetCustomerReservationAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (customerReservations, count) = string.IsNullOrEmpty(searchKey) ? await CustomerReservationService.GetAsync(skip, top, ct) : await CustomerReservationService.GetAsync(skip, top, searchKey, ct);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

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
                    Severity = NotificationSeverity.Info,
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
                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "R" }, { "TITLE", "Está seguro que desea cancelar esta reserva?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;
                var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 3);
                await CustomerReservationService.CancelAsync(customerReservation.CustomerReservationId, cancelStatusDocumentType.StatusDocumentTypeId, reason);

                customerReservation.StatusDocumentType = cancelStatusDocumentType;
                customerReservation.StatusDocumentTypeId = cancelStatusDocumentType.StatusDocumentTypeId;

                await GetCustomerReservationAsync(search);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Reserva de artículos",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"La reserva No. {customerReservation.ReservationNumber}, ha sido cancelada correctamente."
                });
                await CustomerReservationGrid.Reload();

                var result = await DialogService.OpenAsync<CustomerReservationSummary>(null, new Dictionary<string, object> { { "Id", customerReservation.CustomerReservationId }, { "NotificationTemplateName", "Customer:Reservation:Cancellation" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                var summaryResult = (bool)result;
                if (summaryResult)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = $"Notificación",
                        Duration = 6000,
                        Detail = $"Se ha enviado un correo al cliente con el detalle de la reserva."
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido cancelar la reserva."
                });
            }
        }

        protected async Task DownloadAsync(MouseEventArgs arg, CustomerReservation customerReservation)
        {
            var templateCode = new[] { 3 }.Contains(customerReservation.StatusDocumentType.StatusOrder) ? "Customer:Reservation:Cancellation" : "Customer:Reservation:Forwarding";
            var result = await DialogService.OpenAsync<CustomerReservationSummary>(null, new Dictionary<string, object> { { "Id", customerReservation.CustomerReservationId }, { "NotificationTemplateName", templateCode } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
            var dialogResult = (bool)result;
            if (dialogResult)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Notificación",
                    Detail = $"Se ha enviado un correo al cliente con el detalle de la reserva."
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

            await GetCustomerReservationAlarmsAsync(args);
            await GetCustomerReservationNotificationsAsync(args);
        }

        protected async Task GetCustomerReservationNotificationsAsync(CustomerReservation args)
        {
            var customerReservationNotificationsResult = await CustomerReservationNotificationService.GetByCustomerReservationIdAsync(args.CustomerReservationId);
            if (customerReservationNotificationsResult != null)
            {
                args.CustomerReservationNotifications = customerReservationNotificationsResult.ToList();
            }
        }

        protected async Task<bool> CanEdit(CustomerReservation customerReservation)
        {
            return Security.IsInRole("Administrador", "Modificación de reservas") && customerReservation.StatusDocumentType.EditMode;
        }

        protected async Task<bool> CanSendToCustomerOrder(CustomerReservation customerReservation)
        {
            return Security.IsInRole("Administrador", "Modificación de pedidos") && customerReservation.StatusDocumentType.EditMode;
        }

        protected async Task SendToCustomerOrder(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/" + args.CustomerReservationId);
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });

        #region Alarms

        protected async Task<bool> CanEditAlarm(bool alarm, int statusOrder)
        {
            if (!alarm)
                return false;

            int[] validStatusOrder = { 1 };
            return Security.IsInRole("Administrador", "Modificación de pedidos") && validStatusOrder.Contains(statusOrder);
        }

        protected async Task DisableAlarm(Application.Services.Models.Alarm alarm)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Está seguro que desea desactivar esta alarma?") == true)
                {
                    await AlarmService.DisableAsync(alarm.AlarmId);

                    alarm.IsActive = false;
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Alarma de pedido",
                        Severity = NotificationSeverity.Success,
                        Detail = $"La alarma ha sido desactivada correctamente."
                    });

                    await alarmsGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido desactivar la alarma."
                });
            }
        }

        protected async Task GetCustomerReservationAlarmsAsync(CustomerReservation args)
        {
            var CustomerOrderAlarmsResult = await AlarmService.GetByDocumentIdAsync(documentType.DocumentTypeId, args.CustomerReservationId);
            if (CustomerOrderAlarmsResult != null)
            {
                alarms = CustomerOrderAlarmsResult.ToList();
            }
        }

        protected async Task AddAlarmButtonClick(MouseEventArgs args)
        {
            try
            {
                dialogResult = null;
                var alarmResult = await DialogService.OpenAsync<AlarmDialog>("Crear alarma para reserva", new Dictionary<string, object> { { "Title", "Creación de alarma" }, { "DocumentTypeId", documentType.DocumentTypeId }, { "DocumentId", customerReservation.CustomerReservationId } });

                if (alarmResult == null)
                    return;

                await GetCustomerReservationAlarmsAsync(customerReservation);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Alarma para reserva de artículos",
                    Detail = $"La alarma para la reserva No. {customerReservation.ReservationNumber} ha sido creada correctamente."
                });
                await alarmsGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido crear la alarma."
                });
            }
        }

        #endregion

        #endregion
    }
}