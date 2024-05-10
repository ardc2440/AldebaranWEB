using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages
{
    public partial class Index : IDisposable
    {
        #region Injections
        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
               
        [Inject]
        public IAlarmService AlarmService { get; set; }

        [Inject]
        public IVisualizedAlarmService VisualizedAlarmService { get; set; }

        [Inject]
        public IVisualizedPurchaseOrderTransitAlarmService VisualizedPurchaseOrderTransitAlarmService { get; set; }

        [Inject]
        public IPurchaseOrderNotificationService PurchaseOrderNotificationService { get; set; }


        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        #endregion

        #region Parameters

        #endregion

        #region Global Variables

        protected List<MinimumQuantityArticle> minimumQuantityArticles = new List<MinimumQuantityArticle>();
        protected LocalizedDataGrid<MinimumQuantityArticle> minimumQuantityArticlesGrid;
        protected List<OutOfStockArticle> outOfStockArticles = new List<OutOfStockArticle>();
        protected LocalizedDataGrid<OutOfStockArticle> outOfStockArticlesGrid;
        protected List<CustomerReservation> expiredReservations = new List<CustomerReservation>();
        protected LocalizedDataGrid<CustomerReservation> expiredReservationsGrid;
        protected List<Models.ViewModels.Alarm> alarms = new List<Models.ViewModels.Alarm>();
        protected LocalizedDataGrid<Models.ViewModels.Alarm> alarmsGrid;
        protected IEnumerable<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarms = new List<PurchaseOrderTransitAlarm>();
        protected IEnumerable<PurchaseOrderNotification> purchaseOrderNotifications = new List<PurchaseOrderNotification>();
        protected LocalizedDataGrid<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarmsGrid;

        protected int pageSize = 7;
        protected Employee employee;
        protected DocumentType orderDocumentType;
        protected StatusDocumentType pendingStatusOrder;

        List<DataTimer> Timers;
        readonly GridTimer GridTimer = new GridTimer("Dahsboard-GridTimer");
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Timers = TimerPreferenceService.Timers;
            await InitializeGridTimers();
            employee = await DashBoardService.FindByLoginUserIdAsync(Security.User.Id);
            orderDocumentType = await DashBoardService.FindByCodeAsync("O");
            pendingStatusOrder = await DashBoardService.FindByDocumentAndOrderAsync(orderDocumentType.DocumentTypeId, 1);

            await GridData_Update();
        }
        async Task InitializeGridTimers()
        {
            await GridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(GridTimer.Key), async (sender, e) =>
            {
                await InvokeAsync(async () =>
                {
                    GridTimer.IsLoading = true;
                    try
                    {
                        StateHasChanged();
                        await GridData_Update();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Unable to update data for Dashboard Data");
                        NotificationService.Notify(new NotificationMessage
                        {
                            Summary = "Actualización de información",
                            Severity = NotificationSeverity.Error,
                            Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                        });
                    }
                    finally
                    {
                        GridTimer.IsLoading = false;
                        StateHasChanged();
                    }
                });
            });
        }
        private async Task GridData_Update()
        {
            GridTimer.LastUpdate = DateTime.Now;
            Console.WriteLine($"{GridTimer.LastUpdate}");
            var detailInTransit = await DashBoardService.GetTransitDetailOrdersAsync(pendingStatusOrder.StatusDocumentTypeId);
            var itemReferences = await DashBoardService.GetAllReferencesWithMinimumQuantityAsync();
            await UpdateMinimumQuantitiesAsync(detailInTransit.ToList(), itemReferences.ToList());
            await UpdateItemsOutOfStockAsync(detailInTransit.ToList(), itemReferences.ToList());
            await UpdateExpiredReservationsAsync();
            await UpdateUserAlarmsAsync();
            await UpdatePurchaseOrderTransitAlarmsAsync();
        }
        async Task UpdateMinimumQuantitiesAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(itemReferences, detailInTransit);
            await minimumQuantityArticlesGrid.Reload();
        }
        async Task UpdateItemsOutOfStockAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, detailInTransit);
            await outOfStockArticlesGrid.Reload();
        }
        async Task UpdateExpiredReservationsAsync(CancellationToken ct = default)
        {
            expiredReservations = (await DashBoardService.GetExpiredReservationsAsync(ct)).ToList();
            await expiredReservationsGrid.Reload();
        }
        async Task UpdateUserAlarmsAsync(CancellationToken ct = default)
        {
            var alarmList = await DashBoardService.GetByEmployeeIdAsync(employee.EmployeeId, ct);
            alarms = await Models.ViewModels.Alarm.GetAlarmsListAsync(alarmList.ToList(), AlarmService);
            await alarmsGrid.Reload();
        }

        async Task UpdatePurchaseOrderTransitAlarmsAsync(CancellationToken ct = default)
        {
            purchaseOrderTransitAlarms = await DashBoardService.GetAllTransitAlarmAsync(employee.EmployeeId, ct);
            await purchaseOrderTransitAlarmsGrid.Reload(); 
        }

        #endregion

        #region Events
               
        protected async Task OpenCustomerReservation(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/view/" + args.CustomerReservationId);
        }
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task DisableAlarm(Models.ViewModels.Alarm args)
        {
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                await UpdateUserAlarmsAsync();
            }
        }

        protected async Task DisablePurchaseOrderTransitAlarm(PurchaseOrderTransitAlarm args)
        {
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedPurchaseOrderTransitAlarmService.AddAsync(new VisualizedPurchaseOrderTransitAlarm { PurchaseOrderTransitAlarmId = args.PurchaseOrderTransitAlarmId, EmployeeId = employee.EmployeeId, VisualizedDate = System.DateTime.Now });
                await UpdatePurchaseOrderTransitAlarmsAsync();
            }
        }

        private async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }

        #endregion

        #region Notifications
        protected LocalizedDataGrid<PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;
        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }

        protected async Task GetChildData(PurchaseOrderTransitAlarm args)
        {
            var alarm = args;

            var notificationsResult = await PurchaseOrderNotificationService.GetByModifiedPurchaseOrder(args.ModifiedPurchaseOrder.ModifiedPurchaseOrderId);

            if (notificationsResult != null)
            {
                purchaseOrderNotifications = notificationsResult.ToList();                
            }
        }
        #endregion

        public void Dispose()
        {
            GridTimer.Dispose();
        }
    }
}