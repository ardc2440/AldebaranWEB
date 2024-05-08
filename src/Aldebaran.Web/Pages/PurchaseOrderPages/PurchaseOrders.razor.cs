using Aldebaran.Application.Services;
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
        protected IPurchaseOrderNotificationService PurchaseOrderNotificationService { get; set; }

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
                await GetPurchaseOrdersAsync();
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
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
                    Detail = $"Orden de compra {purchaseOrder.OrderNumber} ha sido confirmada correctamente."
                });
                return;
            }
            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Orden de compra",
                Severity = NotificationSeverity.Success,
                Detail = $"Orden de compra {purchaseOrder.OrderNumber} ha sido creada correctamente."
            });
        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        #region PurchaseOrder
        async Task GetPurchaseOrdersAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            PurchaseOrdersList = string.IsNullOrEmpty(searchKey) ? await PurchaseOrderService.GetAsync(ct) : await PurchaseOrderService.GetAsync(searchKey, ct);
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
            NavigationManager.NavigateTo($"edit-purchase-order/{purchaseOrder.PurchaseOrderId}");
        }
        protected async Task ConfirmPurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            NavigationManager.NavigateTo($"confirm-purchase-order/{purchaseOrder.PurchaseOrderId}");
        }
        protected async Task CancelPurchaseOrder(MouseEventArgs args, ServiceModel.PurchaseOrder purchaseOrder)
        {
            try
            {
                var reasonResult = await DialogService.OpenAsync<CancellationReasonDialog>("Confirmar cancelación", new Dictionary<string, object> { { "DOCUMENT_TYPE_CODE", "O" }, { "TITLE", "Está seguro que desea cancelar esta orden de compra?" } });
                if (reasonResult == null)
                    return;
                var reason = (ServiceModel.Reason)reasonResult;
                await PurchaseOrderService.CancelAsync(purchaseOrder.PurchaseOrderId, reason);
                await GetPurchaseOrdersAsync();
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Orden de compra",
                    Severity = NotificationSeverity.Success,
                    Detail = $"Orden de compra ha sido cancelada correctamente."
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
                    Detail = $"No se ha podido agregar la actividad."
                });
            }
        }
        protected async Task EditPurchaseOrderActivity(ServiceModel.PurchaseOrderActivity args, ServiceModel.PurchaseOrder data)
        {
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
                    Detail = $"No se ha podido actualizar la actividad."
                });
            }
        }
        protected async Task DeletePurchaseOrderActivity(MouseEventArgs args, ServiceModel.PurchaseOrder data, ServiceModel.PurchaseOrderActivity purchaseOrderActivity)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta actividad?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == true)
            {
                try
                {
                    await PurchaseOrderActivityService.DeleteAsync(purchaseOrderActivity.PurchaseOrderActivityId);
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Actividad",
                        Severity = NotificationSeverity.Success,
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
                dialogResult = null;
                var alarmResult = await DialogService.OpenAsync<AlarmDialog>("Crear alarma para pedido", new Dictionary<string, object> { { "Title", "Creación de alarma" }, { "DocumentTypeId", documentType.DocumentTypeId }, { "DocumentId", PurchaseOrder.PurchaseOrderId } });

                if (alarmResult == null)
                    return;

                await GetPurchaseOrderAlarmsAsync(PurchaseOrder);

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = $"Alarma para orden de compra",
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
                    Detail = $"No se ha podido crear la alarma."
                });
            }
        }

        #endregion

        #region Notifications
        protected RadzenDataGrid<ServiceModel.PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;
        #endregion

        #endregion
    }
}