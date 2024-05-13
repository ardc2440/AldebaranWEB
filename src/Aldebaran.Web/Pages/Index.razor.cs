using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
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
        public IPurchaseOrderService PurchaseOrderService { get; set; }

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

        [Inject]
        public IOptions<AppSettings> Settings { get; set; }


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
        protected LocalizedDataGrid<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarmsGrid;

        protected IEnumerable<PurchaseOrderNotification> purchaseOrderNotifications = new List<PurchaseOrderNotification>();
        protected LocalizedDataGrid<PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;

        protected IEnumerable<PurchaseOrder> purchaseOrderExpirations = new List<PurchaseOrder>();
        protected LocalizedDataGrid<PurchaseOrder> purchaseOrderExpirationsGrid;

        protected IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> customerOrdersAffected = new List<CustomerOrderAffectedByPurchaseOrderUpdate>();
        protected LocalizedDataGrid<CustomerOrderAffectedByPurchaseOrderUpdate> customerOrdersAffectedGrid;

        protected IEnumerable<CustomerOrder> expiredCustomerOrders = new List<CustomerOrder>();
        protected LocalizedDataGrid<CustomerOrder> expiredCustomerOrdersGrid;


        protected int pageSize = 7;
        protected Employee employee;
        protected DocumentType orderDocumentType;
        protected StatusDocumentType pendingStatusOrder;

        protected bool generalAlertVisible = false;
        protected bool minimumAlertVisible = false;
        protected bool outOfStockAlertVisible = false;
        protected bool expiredReservationsAlertVisible = false;
        protected bool alarmsAlertVisible = false;
        protected bool purchaseAlarmsAlertVisible = false;
        protected bool expiredPurchasesAlertVisible = false;
        protected bool expiredCustomerOrdersAlertVisible = false;

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

        #endregion

        #region Events

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
            await UpdatePurchaseOrderExpirationsAsync();
            await UpdateExpiredCustomerOrdersAsync();

            if (minimumAlertVisible || outOfStockAlertVisible || expiredReservationsAlertVisible || alarmsAlertVisible ||
                purchaseAlarmsAlertVisible || expiredPurchasesAlertVisible || expiredCustomerOrdersAlertVisible) { generalAlertVisible = true; }
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        private async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }

        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }

        public void Dispose()
        {
            GridTimer.Dispose();
        }

        protected async Task GeneralAlertClick()
        {
            generalAlertVisible = false;
            minimumAlertVisible = false;
            outOfStockAlertVisible = false;
            expiredReservationsAlertVisible = false;            
            alarmsAlertVisible = false;
            purchaseAlarmsAlertVisible = false;
            expiredPurchasesAlertVisible = false;
            expiredCustomerOrdersAlertVisible = false;
        }
        protected async Task MinimumAlertClick()
        {
            minimumAlertVisible = false;
        }
        protected async Task StockAlertClick()
        {
            outOfStockAlertVisible = false;
        }
        protected async Task ReservationAlertClick()
        {
            expiredReservationsAlertVisible = false;
        }
        protected async Task AlarmAlertClick()
        {
            alarmsAlertVisible = false;
        }
        protected async Task PurchaseAlarmAlertClick()
        {
            purchaseAlarmsAlertVisible = false;
        }
        protected async Task ExpiredPurchaseAlertClick()
        {
            expiredPurchasesAlertVisible = false;
        }
        protected async Task ExpiredCustomerOrderAlertClick()
        {
            expiredCustomerOrdersAlertVisible = false;
        }

        #region MinimumQuantities
        async Task UpdateMinimumQuantitiesAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            var quantityData = minimumQuantityArticles.Count;
            minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(itemReferences, detailInTransit);
            minimumAlertVisible = minimumQuantityArticles.Count != quantityData;

            await minimumQuantityArticlesGrid.Reload();
        }
        #endregion

        #region ItemsOutOfStock
        async Task UpdateItemsOutOfStockAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            var quantityData = outOfStockArticles.Count;
            outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, detailInTransit);
            outOfStockAlertVisible= quantityData != outOfStockArticles.Count;
            await outOfStockArticlesGrid.Reload();
        }
        #endregion

        #region ExpiredReservations
        async Task UpdateExpiredReservationsAsync(CancellationToken ct = default)
        {
            var quantityExpired = expiredReservations.Count;
            expiredReservations = (await DashBoardService.GetExpiredReservationsAsync(ct)).ToList();
            expiredReservationsAlertVisible = quantityExpired != expiredReservations.Count;
            await expiredReservationsGrid.Reload();
        }

        protected async Task OpenCustomerReservation(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/view/" + args.CustomerReservationId);
        }
        #endregion

        #region ExpiredCustomerOrders
        async Task UpdateExpiredCustomerOrdersAsync(CancellationToken ct = default)
        {
            var quantityExpired = expiredCustomerOrders.ToList().Count;
            expiredCustomerOrders = (await DashBoardService.GetExpiredCustomerOrdersAsync(ct)).ToList();
            expiredCustomerOrdersAlertVisible = quantityExpired != expiredCustomerOrders.ToList().Count;
            await expiredCustomerOrdersGrid.Reload();
        }
        #endregion

        #region UserAlarms
        async Task UpdateUserAlarmsAsync(CancellationToken ct = default)
        {
            var quantityAlarms = alarms.Count;
            var alarmList = await DashBoardService.GetByEmployeeIdAsync(employee.EmployeeId, ct);
            alarms = await Models.ViewModels.Alarm.GetAlarmsListAsync(alarmList.ToList(), AlarmService);
            alarmsAlertVisible = quantityAlarms != alarms.Count;
            await alarmsGrid.Reload();
        }

        protected async Task DisableAlarm(Models.ViewModels.Alarm args)
        {
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                await UpdateUserAlarmsAsync();
            }
        }
        #endregion

        #region PurchaseOrderTransitAlarms
        async Task UpdatePurchaseOrderTransitAlarmsAsync(CancellationToken ct = default)
        {
            var quantity = purchaseOrderTransitAlarms.ToList().Count;
            purchaseOrderTransitAlarms = await DashBoardService.GetAllTransitAlarmAsync(employee.EmployeeId, ct);
            purchaseAlarmsAlertVisible = quantity != purchaseOrderTransitAlarms.ToList().Count;
            await purchaseOrderTransitAlarmsGrid.Reload();
        }

        protected async Task DisablePurchaseOrderTransitAlarm(PurchaseOrderTransitAlarm args)
        {
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedPurchaseOrderTransitAlarmService.AddAsync(new VisualizedPurchaseOrderTransitAlarm { PurchaseOrderTransitAlarmId = args.PurchaseOrderTransitAlarmId, EmployeeId = employee.EmployeeId, VisualizedDate = System.DateTime.Now });
                await UpdatePurchaseOrderTransitAlarmsAsync();
            }
        }

        protected async Task GetAlarmChildData(PurchaseOrderTransitAlarm args)
        {
            var notificationsResult = await PurchaseOrderNotificationService.GetByModifiedPurchaseOrder(args.ModifiedPurchaseOrder.ModifiedPurchaseOrderId);

            if (notificationsResult != null)
            {
                purchaseOrderNotifications = notificationsResult.ToList();
            }
        }
        #endregion

        #region PurchaseOrderExpirations

        public async Task UpdatePurchaseOrderExpirationsAsync(CancellationToken ct = default)
        {
            var quantity = purchaseOrderExpirations.ToList().Count;
            purchaseOrderExpirations = await DashBoardService.GetPurchaseOrderExpirationsAsync(Settings.Value.PurchaseOrderWhiteFlag, ct);
            expiredPurchasesAlertVisible = quantity != purchaseOrderExpirations.ToList().Count;
            await purchaseOrderExpirationsGrid.Reload();
        }

        protected async Task GetExpiredPurchaseOrderChildData(PurchaseOrder args)
        {
            var customerOrderResult = await PurchaseOrderService.GetAffectedCustomerOrders(args.PurchaseOrderId);

            if (customerOrderResult != null)
            {
                customerOrdersAffected = customerOrderResult.ToList();
            }
        }

        protected bool CanExpand(PurchaseOrder data)
        {
            var days = (int)(data.ExpectedReceiptDate - System.DateTime.Today).Days;

            return days <= Settings.Value.PurchaseOrderRedFlag;
        }

        protected async void RowRender(RowRenderEventArgs<PurchaseOrder> args)
        {
            args.Expandable = CanExpand(args.Data);
        }

        protected async void CellRender(DataGridCellRenderEventArgs<PurchaseOrder> args)
        {
            var days = (int)(args.Data.ExpectedReceiptDate - System.DateTime.Today).Days;
            if (days <= Settings.Value.PurchaseOrderRedFlag)
            {
                args.Attributes.Add("style", $"background-color:var(--rz-danger-light)");
            }
            else
            {
                if (days <= Settings.Value.PurchaseOrderYellowFlag)
                {
                    args.Attributes.Add("style", $"background-color:var(--rz-warning-light)");
                }
            }
        }
        #endregion

        #endregion
    }
}