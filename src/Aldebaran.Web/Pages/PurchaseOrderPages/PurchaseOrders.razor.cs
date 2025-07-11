using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class PurchaseOrders
    {

        #region Injections
        [Inject]
        protected ILogger<PurchaseOrders> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        protected IPurchaseOrderActivityService PurchaseOrderActivityService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        protected IProviderReferenceService ProviderReferenceService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected IAlarmService AlarmService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IPurchaseOrderNotificationService PurchaseOrderNotificationService { get; set; }

        [Inject]
        protected ICancellationRequestService CancellationRequestService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public string PURCHASE_ORDER_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;
        #endregion

        #region Variables
        protected DialogResult dialogResult;
        protected ServiceModel.PurchaseOrder PurchaseOrder;
        protected IEnumerable<ServiceModel.PurchaseOrder> PurchaseOrdersList;
        protected RadzenDataGrid<ServiceModel.PurchaseOrder> PurchaseOrderGrid;
        protected string search = "";
        protected bool isLoadingInProgress;

        protected int skip = 0;
        protected int top = 0;
        protected int count;

        protected ServiceModel.DocumentType documentType;
        protected IEnumerable<ServiceModel.Alarm> alarms;
        protected LocalizedDataGrid<ServiceModel.Alarm> alarmsGrid;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                documentType = await DocumentTypeService.FindByCodeAsync("O");

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
            await GetPurchaseOrdersAsync(search);
        }

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (PURCHASE_ORDER_ID == null)
                return;
            var valid = int.TryParse(PURCHASE_ORDER_ID, out var purchaseOrderID);
            if (!valid)
                return;
            var purchaseOrder = await PurchaseOrderService.FindAsync(purchaseOrderID, ct);
            if (purchaseOrder == null)
                return;
            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Orden de compra",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Orden de compra {purchaseOrder.OrderNumber} ha sido actualizada correctamente."
                });
                return;
            }
            if (Action == "confirm")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Orden de compra",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Orden de compra {purchaseOrder.OrderNumber} ha sido confirmada correctamente."
                });
                return;
            }
            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Orden de compra",
                Severity = NotificationSeverity.Success,
                Duration = 6000,
                Detail = $"Orden de compra {purchaseOrder.OrderNumber} ha sido creada correctamente."
            });
        }
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);
        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion

        #region PurchaseOrder
        async Task GetPurchaseOrdersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (PurchaseOrdersList, count) = string.IsNullOrEmpty(searchKey) ? await PurchaseOrderService.GetAsync(skip, top, ct) : await PurchaseOrderService.GetAsync(skip, top, searchKey, ct);
        }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await PurchaseOrderGrid.GoToPage(0);
            await GetPurchaseOrdersAsync(search);
        }
        protected async Task AddPurchaseOrder(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-purchase-order");
        }
        protected async Task GetChildData(ServiceModel.PurchaseOrder args)
        {
            PurchaseOrder = args;
            await Task.Yield();
            var details = await PurchaseOrderDetailService.GetByPurchaseOrderIdAsync(args.PurchaseOrderId);
            args.PurchaseOrderDetails = details.ToList();
            var activities = await PurchaseOrderActivityService.GetByPurchaseOrderIdAsync(args.PurchaseOrderId);
            args.PurchaseOrderActivities = activities.ToList();
            var notifications = await PurchaseOrderNotificationService.GetByPurchaseOrderId(args.PurchaseOrderId);
            args.PurchaseOrderNotifications = notifications.ToList();
            await GetPurchaseOrderAlarmsAsync(args);
        }
        protected async Task EditPurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            if (!await HasCancellationRequest(purchaseOrder, "Existe una solicitud en estudio para cancelar esta orden de compra. No se puede modificar."))
                NavigationManager.NavigateTo($"edit-purchase-order/{purchaseOrder.PurchaseOrderId}");
        }

        protected async Task<bool> HasCancellationRequest(PurchaseOrder purchaseOrder, string message)
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("C");

            if (await CancellationRequestService.ExistsAnyPendingRequestAsync(documentType.DocumentTypeId, purchaseOrder.PurchaseOrderId))
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = $"Alerta",
                    Duration = 6000,
                    Detail = message,
                    Style = "background-color: #db2001; color: white; font-size: 16px; padding: 1px;"
                });
                return true;
            }
            return false;
        }

        protected async Task ConfirmPurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            if (!await HasCancellationRequest(purchaseOrder, "Existe una solicitud en estudio para cancelar esta orden de compra. No se puede confirmar."))
                NavigationManager.NavigateTo($"confirm-purchase-order/{purchaseOrder.PurchaseOrderId}");
        }
        protected async Task CancelPurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            try
            {
                if (await HasCancellationRequest(purchaseOrder, "Ya existe una solicitud en estudio para cancelar esta orden de compra.")) 
                    return;

                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar solicitud de cancelaci�n", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "O" }, { "TITLE", "Est� seguro que desea solicitar cancelar esta orden de compra?" } });
                if (reasonResult == null) return;

                var reason = (ServiceModel.Reason)reasonResult;

                var cancellationDocumentType = await DocumentTypeService.FindByCodeAsync("C");

                await CancellationRequestService.AddAsync(new ServiceModel.CancellationRequest
                {
                    RequestEmployeeId = reason.EmployeeId,
                    DocumentNumber = purchaseOrder.PurchaseOrderId,
                    DocumentType = cancellationDocumentType,
                    StatusDocumentTypeId = (await StatusDocumentTypeService.FindByDocumentAndOrderAsync(cancellationDocumentType.DocumentTypeId, 1)).StatusDocumentTypeId
                }, reason);

                // OJO Esto pasa a hacerlo al momento de aprobarse la solicitud de cancelaci�n
                // await PurchaseOrderService.CancelAsync(purchaseOrder.PurchaseOrderId, reason);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Orden de compra",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Solicitud de cancelaci�n para la orden de compra No. {purchaseOrder.OrderNumber} enviada correctamente."
                });

                await PurchaseOrderGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(CancelPurchaseOrder));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido cancelar la orden de compra."
                });
            }
        }
        #endregion

        #region PurchaseOrderActivity

        protected async Task<string> GetReferenceHint(ServiceModel.ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected RadzenDataGrid<ServiceModel.PurchaseOrderActivity> PurchaseOrderActivitiesDataGrid;
        protected async Task AddPurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrder data)
        {
            if (await HasCancellationRequest(data, "Existe una solicitud en estudio para cancelar esta orden de compra. No se pueden agregar actividades."))
                return;

            var result = await DialogService.OpenAsync<AddPurchaseOrderActivity>("Nueva actividad");
            if (result == null)
                return;
            var activityResult = (ServiceModel.PurchaseOrderActivity)result;
            try
            {
                var activity = new ServiceModel.PurchaseOrderActivity
                {
                    PurchaseOrderId = data.PurchaseOrderId,
                    ExecutionDate = activityResult.ExecutionDate,
                    ActivityDescription = activityResult.ActivityDescription,
                    CreationDate = DateTime.Now,
                    EmployeeId = activityResult.EmployeeId,
                    ActivityEmployeeId = activityResult.ActivityEmployeeId
                };
                await PurchaseOrderActivityService.AddAsync(activity);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Actividad",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Actividad ha sido agregada correctamente a la orden {data.OrderNumber}."
                });
                await GetChildData(data);
                await PurchaseOrderActivitiesDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(AddPurchaseOrderActivity));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido agregar la actividad."
                });
            }
        }
        protected async Task EditPurchaseOrderActivity(ServiceModel.PurchaseOrderActivity args, ServiceModel.PurchaseOrder data)
        {
            if (await HasCancellationRequest(data, "Existe una solicitud en estudio para cancelar esta orden de compra. No se pueden modificar actividades."))
                return;

            var result = await DialogService.OpenAsync<EditPurchaseOrderActivity>("Actualizar actividad", new Dictionary<string, object> { { "PURCHASE_ORDER_ACTIVITY_ID", args.PurchaseOrderActivityId } });
            if (result == null)
                return;
            var activityResult = (ServiceModel.PurchaseOrderActivity)result;
            try
            {
                await PurchaseOrderActivityService.UpdateAsync(args.PurchaseOrderActivityId, activityResult);
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Actividad",
                    Severity = NotificationSeverity.Success,
                    Duration = 6000,
                    Detail = $"Actividad ha sido actualizada correctamente."
                });
                await GetChildData(data);
                await PurchaseOrderActivitiesDataGrid.Reload();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(EditPurchaseOrderActivity));
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Duration = 6000,
                    Detail = $"No se ha podido actualizar la actividad."
                });
            }
        }
        protected async Task DeletePurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrder data, ServiceModel.PurchaseOrderActivity purchaseOrderActivity)
        {
            if (await HasCancellationRequest(data, "Existe una solicitud en estudio para cancelar esta orden de compra. No se pueden eliminar actividades."))
                return;

            if (await DialogService.Confirm("Est� seguro que desea eliminar esta actividad?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminaci�n") == true)
            {
                try
                {
                    await PurchaseOrderActivityService.DeleteAsync(purchaseOrderActivity.PurchaseOrderActivityId);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Actividad",
                        Severity = NotificationSeverity.Success,
                        Duration = 6000,
                        Detail = $"Actividad ha sido eliminada correctamente."
                    });
                    await GetChildData(data);
                    await PurchaseOrderActivitiesDataGrid.Reload();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, nameof(DeletePurchaseOrderActivity));
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = $"Error",
                        Duration = 6000,
                        Detail = $"No se ha podido eliminar la actividad."
                    });
                }
            }
        }
        #endregion

        #region PurchaseOrderDetail
        protected RadzenDataGrid<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetailsDataGrid;
        #endregion

        #region Alarms

        protected async Task DisableAlarm(Application.Services.Models.Alarm alarm)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Est� seguro que desea desactivar esta alarma?") == true)
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

        protected async Task GetPurchaseOrderAlarmsAsync(ServiceModel.PurchaseOrder args)
        {
            var CustomerOrderAlarmsResult = await AlarmService.GetByDocumentIdAsync(documentType.DocumentTypeId, args.PurchaseOrderId);
            if (CustomerOrderAlarmsResult != null)
            {
                alarms = CustomerOrderAlarmsResult.ToList();
            }
        }

        protected async Task AddAlarmButtonClick(MouseEventArgs args)
        {
            try
            {
                if (await HasCancellationRequest(PurchaseOrder, "Existe una solicitud en estudio para cancelar esta orden de compra. No se pueden agregar alarmas."))
                    return;

                dialogResult = null;
                var alarmResult = await DialogService.OpenAsync<AlarmDialog>("Crear alarma para pedido", new Dictionary<string, object> { { "Title", "Creaci�n de alarma" }, { "DocumentTypeId", documentType.DocumentTypeId }, { "DocumentId", PurchaseOrder.PurchaseOrderId } });

                if (alarmResult == null)
                    return;

                await GetPurchaseOrderAlarmsAsync(PurchaseOrder);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Alarma para orden de compra",
                    Duration = 6000,
                    Detail = $"La alarma para la orden de compra No. {PurchaseOrder.OrderNumber} ha sido creada correctamente."
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

        #region Notifications
        protected LocalizedDataGrid<ServiceModel.PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;
        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }
        #endregion


    }
}