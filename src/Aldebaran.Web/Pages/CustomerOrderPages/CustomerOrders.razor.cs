using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class CustomerOrders
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
        protected ICustomerOrderService CustomerOrderService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected ICustomerOrderActivityService CustomerOrderActivityService { get; set; }

        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }

        [Inject]
        protected ICustomerOrderActivityDetailService CustomerOrderActivityDetailService { get; set; }

        [Inject]
        protected ICustomerOrderNotificationService CustomerOrderNotificationService { get; set; }

        [Inject]
        protected ICustomerOrderInProcessDetailService CustomerOrderInProcessDetailService { get; set; }

        [Inject]
        protected ICustomerOrdersInProcessService CustomerOrdersInProcessService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentService CustomerOrderShipmentService { get; set; }

        [Inject]
        protected ICustomerOrderShipmentDetailService CustomerOrderShipmentDetailService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IAlarmService AlarmService { get; set; }

        [Inject]
        protected ICancellationRequestService CancellationRequestService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string CUSTOMER_ORDER_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Global Variables
        protected DialogResult dialogResult;
        protected DocumentType documentType;
        protected IEnumerable<CustomerOrder> customerOrders;
        protected LocalizedDataGrid<CustomerOrder> CustomerOrdersGrid;
        protected CustomerOrder customerOrder;
        protected CustomerOrderActivity customerOrderActivity;
        protected IEnumerable<CustomerOrderDetail> CustomerOrderDetails;
        protected LocalizedDataGrid<CustomerOrderDetail> CustomerOrderDetailsDataGrid;
        protected LocalizedDataGrid<CustomerOrderActivity> CustomerOrderActivitiesDataGrid;
        protected LocalizedDataGrid<CustomerOrderActivityDetail> CustomerOrderActivityDetailsDataGrid;
        protected LocalizedDataGrid<CustomerOrderNotification> CustomerOrderNotificationsDataGrid;        
        protected IEnumerable<CustomerOrderInProcessDetail> customerOrderInProcessDetails;
        protected LocalizedDataGrid<CustomerOrdersInProcess> CustomerOrderInProcessesDataGrid;
        protected LocalizedDataGrid<CustomerOrderInProcessDetail> CustomerOrderInProcessDetailDataGrid;
        protected IEnumerable<CustomerOrderShipmentDetail> customerOrderShipmentDetails;
        protected LocalizedDataGrid<CustomerOrderShipment> CustomerOrderShipmentDataGrid;
        protected LocalizedDataGrid<CustomerOrderShipmentDetail> CustomerOrderShipmentDetailDataGrid;

        protected string search = "";
        protected bool isLoadingInProgress;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        protected IEnumerable<Application.Services.Models.Alarm> alarms;
        protected LocalizedDataGrid<Application.Services.Models.Alarm> alarmsGrid;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                documentType = await DocumentTypeService.FindByCodeAsync("P");
                isLoadingInProgress = true;
                await Task.Yield();

                await DialogResultResolver();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
            await GetCustomerOrdersAsync(search);            
        }

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (CUSTOMER_ORDER_ID == null)
                return;

            var valid = int.TryParse(CUSTOMER_ORDER_ID, out var customerOrderId);
            if (!valid)
                return;

            var customerOrder = await CustomerOrderService.FindAsync(customerOrderId, ct);
            if (customerOrder == null)
                return;

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"El pedido {customerOrder.OrderNumber} ha sido actualizado correctamente."
                });
                return;
            }

            if (Action == "create-activity")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"La actividad para el pedido {customerOrder.OrderNumber} ha sido creada correctamente."
                });
                return;
            }

            if (Action == "edit-activity")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"La actividad para el pedido {customerOrder.OrderNumber} ha sido actualizada correctamente."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Pedido de artículos",
                Severity = NotificationSeverity.Success,
                Duration = 6000,
                Detail = $"El pedido ha sido creado correctamente, con el consecutivo {customerOrder.OrderNumber}."
            });
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task GetCustomerOrdersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (customerOrders, count) = string.IsNullOrEmpty(searchKey) ? await CustomerOrderService.GetAsync(skip, top, ct: ct) : await CustomerOrderService.GetAsync(skip, top, searchKey, ct: ct);
        }

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await CustomerOrdersGrid.GoToPage(0);

            await GetCustomerOrdersAsync(search);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-customer-order");
        }

        protected async Task EditRow(CustomerOrder args)
        {
            if (!await HasCancellationRequest(args, "No se puede modificar.") && !await HasClosedRequest(args, "No se puede modificar."))
                NavigationManager.NavigateTo("edit-customer-order/" + args.CustomerOrderId);
        }

        protected async Task AddActivityButtonClick(MouseEventArgs args)
        {
            if (!await HasCancellationRequest(customerOrder, "No se pueden agregar actividades.") && !await HasClosedRequest(customerOrder, "No se pueden agregar actividades."))
                NavigationManager.NavigateTo("add-customer-order-activity/" + customerOrder.CustomerOrderId);
        }

        protected async Task EditActivityRow(CustomerOrderActivity args)
        {
            var customerOrder = await CustomerOrderService.FindAsync(args.CustomerOrderId);

            if (!await HasCancellationRequest(customerOrder, "No se pueden editar actividades.") && !await HasClosedRequest(customerOrder, "No se pueden editar actividades."))
                NavigationManager.NavigateTo("edit-customer-order-activity/" + args.CustomerOrderActivityId);
        }

        protected async Task DeleteActivity(MouseEventArgs arg, CustomerOrderActivity customerOrderActivity)
        {
            try
            {
                var customerOrder = await CustomerOrderService.FindAsync(customerOrderActivity.CustomerOrderId);

                if (await HasCancellationRequest(customerOrder, "No se pueden eliminar actividades.") || await HasClosedRequest(customerOrder, "No se pueden eliminar actividades."))
                    return;

                dialogResult = null;

                if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?") == true)
                {
                    await CustomerOrderActivityService.DeleteAsync(customerOrderActivity.CustomerOrderActivityId);
                    await GetCustomerOrderActivitiesAsync(customerOrder);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Actividad de pedido",
                        Severity = NotificationSeverity.Success,
                        Duration = 6000,
                        Detail = $"La Actividad del pedido ha sido eliminada correctamente."
                    });
                    await CustomerOrderActivitiesDataGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido eliminar la actividad."
                });
            }
        }

        protected async Task CancelCustomerOrder(MouseEventArgs arg, CustomerOrder customerOrder)
        {
            try
            {
                if (await HasClosedRequest(customerOrder, "") || await HasCancellationRequest(customerOrder, ""))
                    return;

                dialogResult = null;
                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "P" }, { "TITLE", "Está seguro que desea cancelar este pedido?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;

                await CancellationRequestService.AddAsync(new CancellationRequest
                {
                    RequestEmployeeId = reason.EmployeeId,
                    DocumentNumber = customerOrder.CustomerOrderId,
                    DocumentType = documentType,
                    StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1)).StatusDocumentTypeId
                }, reason);

// OJO Esto pasa a hacerlo al momento de aprobarse la solicitud de cancelación

                //var cancelStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 6);
                //await CustomerOrderService.CancelAsync(customerOrder.CustomerOrderId, cancelStatusDocumentType.StatusDocumentTypeId, reason);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Pedido de artículos",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Solicitud de cancelación para el pedido No. {customerOrder.OrderNumber} enviada correctamente."
                });
                await CustomerOrdersGrid.Reload();

// OJO Esto pasa a hacerlo al momento de aprobarse la solicitud de cancelación

                //var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", "Customer:Order:Cancellation" } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
                //var summaryResult = (bool)result;
                //if (summaryResult)
                //{
                //    NotificationService.Notify(new NotificationMessage
                //    {
                //        Severity = NotificationSeverity.Success,
                //        Summary = $"Notificación",
                //        Duration = 6000,
                //        Detail = $"Se ha enviado un correo al cliente con el detalle del pedido."
                //    });
                //}

            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido cancelar el pedido."
                });
            }
        }
        protected async Task DownloadAsync(MouseEventArgs arg, CustomerOrder customerOrder)
        {
            var templateCode = new[] { 5, 6 }.Contains(customerOrder.StatusDocumentType.StatusOrder) ? "Customer:Order:Cancellation" : "Customer:Order:Forwarding";
            var result = await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrder.CustomerOrderId }, { "NotificationTemplateName", templateCode } }, options: new DialogOptions { ShowTitle = false, ShowClose = false, CloseDialogOnEsc = false, CloseDialogOnOverlayClick = false, Width = "800px" });
            var dialogResult = (bool)result;
            if (dialogResult)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Notificación",
                    Duration = 6000,
                    Detail = $"Se ha enviado un correo al cliente con el detalle del pedido."
                });
            }
        }
        protected async Task GetOrderDetails(CustomerOrder args)
        {
            var customerOrderDetailsResult = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderDetailsResult != null)
            {
                CustomerOrderDetails = customerOrderDetailsResult.ToList();
            }
        }

        protected async Task GetCustomerOrderActivitiesAsync(CustomerOrder args)
        {
            var customerOrderActivitiesResult = await CustomerOrderActivityService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderActivitiesResult != null)
            {
                args.CustomerOrderActivities = customerOrderActivitiesResult.ToList();
            }
        }

        protected async Task GetChildActivityData(CustomerOrderActivity args)
        {
            customerOrderActivity = args;

            var customerOrderActivityDetailsResult = await CustomerOrderActivityDetailService.GetByCustomerOrderActivityIdAsync(args.CustomerOrderActivityId);
            if (customerOrderActivityDetailsResult != null)
            {
                args.CustomerOrderActivityDetails = customerOrderActivityDetailsResult.ToList();
            }
        }

        protected async Task GetCustomerOrderNotificationsAsync(CustomerOrder args)
        {
            var customerOrderNotificationsResult = await CustomerOrderNotificationService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderNotificationsResult != null)
            {
                args.CustomerOrderNotifications = customerOrderNotificationsResult.ToList();
            }
        }

        protected async Task GetChildData(CustomerOrder args)
        {
            customerOrder = args;

            await GetOrderDetails(args);

            await GetCustomerOrderActivitiesAsync(args);

            await GetCustomerOrderAlarmsAsync(args);

            await GetCustomerOrderNotificationsAsync(args);

            await GetCustomerOrderInProcessAsync(args);

            await GetCustomerOrderShipmentAsync(args);
        }

        protected async Task<bool> CanEdit(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Administrador", "Modificación de pedidos") && customerOrder.StatusDocumentType.EditMode;
        }

        protected async Task<bool> CanCloseCustomerOrder(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Administrador", "Cierre de pedidos") && (customerOrder.StatusDocumentType.StatusOrder == 2 || customerOrder.StatusDocumentType.StatusOrder == 3);
        }

        protected async Task<bool> CanCancel(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Administrador", "Cancelción de pedidos") && (customerOrder.StatusDocumentType.StatusOrder == 1);
        }

        protected async Task CloseCustomerOrder(CustomerOrder args)
        {
            try
            {
                if (await HasCancellationRequest(args, "") || await HasClosedRequest(args, ""))
                    return;

                if ((await CustomerOrderDetailService.GetByCustomerOrderIdAsync(args.CustomerOrderId)).Any(i => i.ProcessedQuantity > 0))
                    throw new Exception("Existen cantidades en proceso");

                dialogResult = null;
                var reasonResult = await DialogService.OpenAsync<CloseCustomerOrderReasonDialog>("Confirmar cierre de pedido", new Dictionary<string, object> { { "TITLE", "Está seguro que desea cerrar este pedido?" } });
                if (reasonResult == null)
                    return;
                var reason = (Reason)reasonResult;

                await CancellationRequestService.AddAsync(new CancellationRequest
                {
                    RequestEmployeeId = reason.EmployeeId,
                    DocumentNumber = args.CustomerOrderId,
                    DocumentType = documentType,
                    StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1)).StatusDocumentTypeId
                }, reason);

// OJO Esto pasa a hacerlo al momento de aprobarse la solicitud de cancelación
                //var closeStatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 5);
                //await CustomerOrderService.CloseAsync(args.CustomerOrderId, closeStatusDocumentType.StatusDocumentTypeId, reason);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Pedido de artículos",
                    Duration = 6000,
                    Detail = $"Solicitud de cierre para el pedido No. {args.OrderNumber} enviada correctamente."
                });

                await CustomerOrdersGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Duration = 6000,
                    Detail = $"No se ha podido cerrar el pedido. <br>"+ex.Message
                });
            }
        }

        protected async Task<bool> CanEditActivities(CustomerOrder customerOrder)
        {
            return Security.IsInRole("Administrador", "Modificación de pedidos","Creación de pedidos") && customerOrder.StatusDocumentType.EditMode;
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });

        protected async Task<bool> HasCancellationRequest(CustomerOrder customerOrder, string message)
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("E");

            if (await CancellationRequestService.ExistsAnyPendingRequestAsync(documentType.DocumentTypeId, customerOrder.CustomerOrderId))
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = $"Alerta",
                    Duration = 6000,
                    Detail = "Existe una solicitud en estudio para cancelar este pedido. " + message,
                    Style = "background-color: #db2001; color: white; font-size: 16px; padding: 1px;"
                });
                return true;
            }
            return false;
        }

        protected async Task<bool> HasClosedRequest(CustomerOrder customerOrder, string message)
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("F");

            if (await CancellationRequestService.ExistsAnyPendingRequestAsync(documentType.DocumentTypeId, customerOrder.CustomerOrderId))
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = $"Alerta",
                    Duration = 6000,
                    Detail = "Existe una solicitud en estudio para cerrar este pedido. "+message,
                    Style = "background-color: #db2001; color: white; font-size: 16px; padding: 1px;"
                });
                return true;
            }
            return false;
        }

        #endregion

        #region Alarms

        protected async Task<bool> CanEditAlarm(bool alarm, int statusOrder)
        {
            if (!alarm)
                return false;

            int[] validStatusOrder = { 1, 2, 3 };
            return Security.IsInRole("Administrador", "Creación de pedidos", "Modificación de pedidos") && validStatusOrder.Contains(statusOrder);
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
                        Duration = 6000,
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
                    Duration = 6000,
                    Detail = $"No se ha podido desactivar la alarma."
                });
            }
        }

        protected async Task GetCustomerOrderAlarmsAsync(CustomerOrder args)
        {
            var CustomerOrderAlarmsResult = await AlarmService.GetByDocumentIdAsync(documentType.DocumentTypeId, args.CustomerOrderId);
            if (CustomerOrderAlarmsResult != null)
            {
                alarms = CustomerOrderAlarmsResult.ToList();
            }
        }

        protected async Task AddAlarmButtonClick(MouseEventArgs args)
        {
            if (await HasCancellationRequest(customerOrder, "No se pueden agregar alarmas.") || await HasClosedRequest(customerOrder, "No se pueden agregar alarmas."))
                return;

            try
            {
                dialogResult = null;
                var alarmResult = await DialogService.OpenAsync<AlarmDialog>("Crear alarma para pedido", new Dictionary<string, object> { { "Title", "Creación de alarma" }, { "DocumentTypeId", documentType.DocumentTypeId }, { "DocumentId", customerOrder.CustomerOrderId } });

                if (alarmResult == null)
                    return;

                await GetCustomerOrderAlarmsAsync(customerOrder);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Alarma para pedido de artículos",
                    Duration = 6000,
                    Detail = $"La alarma para el pedido No. {customerOrder.OrderNumber} ha sido creada correctamente."
                });
                await alarmsGrid.Reload();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido crear la alarma."
                });
            }
        }

        #endregion

        #region CustomerOrderInProcess
        private async Task GetCustomerOrderInProcessAsync(CustomerOrder args)
        {
            var customerOrderinProcessResult = await CustomerOrdersInProcessService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderinProcessResult != null)
            {
                args.CustomerOrdersInProcesses = customerOrderinProcessResult.ToList();
            }
        }
        protected async Task GetChildInProcessData(CustomerOrdersInProcess args)
        {
            customerOrderInProcessDetails = await CustomerOrderInProcessDetailService.GetByCustomerOrderInProcessIdAsync(args.CustomerOrderInProcessId);
        }
        #endregion

        #region CustomerOrderShipment
        private async Task GetCustomerOrderShipmentAsync(CustomerOrder args)
        {
            var customerOrderShipmentResult = await CustomerOrderShipmentService.GetByCustomerOrderIdAsync(args.CustomerOrderId);
            if (customerOrderShipmentResult != null)
            {
                args.CustomerOrderShipments = customerOrderShipmentResult.ToList();
            }
        }
        protected async Task GetChildShipmentData(CustomerOrderShipment args)
        {
            customerOrderShipmentDetails = await CustomerOrderShipmentDetailService.GetByCustomerOrderShipmentIdAsync(args.CustomerOrderShipmentId);
        }
        #endregion

    }
}