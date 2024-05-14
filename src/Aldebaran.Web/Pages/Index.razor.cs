using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
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

        [Inject]
        protected IMemoryCache MemoryCache { get; set; }

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
        private MemoryCacheEntryOptions _cacheEntryOptions;
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            Timers = TimerPreferenceService.Timers;
            await InitializeGridTimers();
            employee = await DashBoardService.FindByLoginUserIdAsync(Security.User.Id);
            orderDocumentType = await DashBoardService.FindByCodeAsync("O");
            pendingStatusOrder = await DashBoardService.FindByDocumentAndOrderAsync(orderDocumentType.DocumentTypeId, 1);
            _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = Settings.Value.SlidingExpirationCache};
            await GridData_Update();
        }

        #endregion

        #region Events

        string GetCacheKey(string key)
        {
            return $"{Security.User.Id}-{key}";
        }

        public async Task UpdateDashBoardCache<T>(string key, List<T> list) where T:class
        {
            MemoryCache.Set(GetCacheKey(key), list, _cacheEntryOptions);
        }

        public async Task<List<T>> GetDashBoardCache<T>(string key) where T:class
        {
            var loggedUserCache = GetCacheKey(key);
            if (!MemoryCache.TryGetValue(loggedUserCache, out List<T> list))
            {
                MemoryCache.Set(loggedUserCache, new List<T>(), _cacheEntryOptions);
                return new List<T>();
            }
            return list ?? new List<T>();
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
            generalAlertVisible = false;
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

        internal static async Task<bool> IsEqual<T>(List<T> first, List<T> second) where T : class
        {
            if (first == null || second == null || first.Count != second.Count)
            {
                return false;
            }

            for (var i = 0; i < first.Count; i++)
            {
                var a = first[i];
                var b = second[i];

                for (var j = 0; j < a.GetType().GetProperties().Count(); j++)
                {
                    var val1 = a.GetType().GetProperties()[j].GetValue(a);
                    var val2 = b.GetType().GetProperties()[j].GetValue(b);

                    if ((val1 == null && val2 != null) || (val1 != null && val2 == null))
                        return false;
                    
                    if (val1 != null && val2 != null && (val1.GetType() == Type.GetType("System.Int32") ||
                                                         val1.GetType() == Type.GetType("System.Int64") ||
                                                         val1.GetType() == Type.GetType("System.Double") ||
                                                         val1.GetType() == Type.GetType("System.Decimal") ||
                                                         val1.GetType() == Type.GetType("System.DateTime") ||
                                                         val1.GetType() == Type.GetType("System.String") ||
                                                         val1.GetType() == Type.GetType("System.Boolean")))
                        if (!val1.Equals(val2))
                            return false;
                }
            }

            return true;
        }

        #region MinimumQuantities
        async Task UpdateMinimumQuantitiesAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            var originalData = await GetDashBoardCache<MinimumQuantityArticle>("MinimumQuantityArticle");

            minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(itemReferences, detailInTransit);
            minimumAlertVisible = !await IsEqual<MinimumQuantityArticle>(minimumQuantityArticles.OrderBy(o => o.ArticleName).ToList(), originalData.OrderBy(o => o.ArticleName).ToList());
            await UpdateDashBoardCache<MinimumQuantityArticle>("MinimumQuantityArticle", minimumQuantityArticles);
            await minimumQuantityArticlesGrid.Reload();
        }
        #endregion

        #region ItemsOutOfStock
        async Task UpdateItemsOutOfStockAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            var originalData = await GetDashBoardCache<OutOfStockArticle>("OutOfStockArticle");  
            outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, detailInTransit);
            outOfStockAlertVisible = !await IsEqual<OutOfStockArticle>(outOfStockArticles.OrderBy(o => o.ArticleName).ToList(), originalData.OrderBy(o => o.ArticleName).ToList());
            await UpdateDashBoardCache<OutOfStockArticle>("OutOfStockArticle", outOfStockArticles);
            await outOfStockArticlesGrid.Reload();
        }
        #endregion

        #region ExpiredReservations
        async Task UpdateExpiredReservationsAsync(CancellationToken ct = default)
        {
            var originalData = await GetDashBoardCache<CustomerReservation>("CustomerReservation"); 

            expiredReservations = (await DashBoardService.GetExpiredReservationsAsync(ct)).ToList();
            expiredReservationsAlertVisible = !await IsEqual<CustomerReservation>(expiredReservations.OrderBy(o => o.CustomerReservationId).ToList(), originalData.OrderBy(o => o.CustomerReservationId).ToList());
            await UpdateDashBoardCache<CustomerReservation>("CustomerReservation", expiredReservations);
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
            var originalData = await GetDashBoardCache<CustomerOrder>("CustomerOrder"); 

            expiredCustomerOrders = (await DashBoardService.GetExpiredCustomerOrdersAsync(ct)).ToList();
            expiredCustomerOrdersAlertVisible = !await IsEqual<CustomerOrder>(expiredCustomerOrders.OrderBy(o => o.CustomerOrderId).ToList(), originalData.OrderBy(o => o.CustomerOrderId).ToList());
            await UpdateDashBoardCache<CustomerOrder>("CustomerOrder", expiredCustomerOrders.ToList());
            await expiredCustomerOrdersGrid.Reload();
        }
        #endregion

        #region UserAlarms
        async Task UpdateUserAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetDashBoardCache<Models.ViewModels.Alarm>("Alarm");

            var alarmList = await DashBoardService.GetByEmployeeIdAsync(employee.EmployeeId, ct);
            alarms = await Models.ViewModels.Alarm.GetAlarmsListAsync(alarmList.ToList(), AlarmService, ct);
            alarmsAlertVisible = !await IsEqual<Models.ViewModels.Alarm>(alarms.OrderBy(o => o.AlarmId).ToList(), originalData.OrderBy(o => o.AlarmId).ToList());
            await UpdateDashBoardCache<Models.ViewModels.Alarm>("Alarm", alarms);
            await alarmsGrid.Reload();
        }

        protected async Task DisableAlarm(Models.ViewModels.Alarm args)
        {
            var alertVisible = alarmsAlertVisible;

            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });                
                await UpdateUserAlarmsAsync();
                alarmsAlertVisible = alertVisible;
            }
        }
        #endregion

        #region PurchaseOrderTransitAlarms
        async Task UpdatePurchaseOrderTransitAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetDashBoardCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm"); 

            purchaseOrderTransitAlarms = await DashBoardService.GetAllTransitAlarmAsync(employee.EmployeeId, ct);
            purchaseAlarmsAlertVisible = !await IsEqual<PurchaseOrderTransitAlarm>(purchaseOrderTransitAlarms.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList(), originalData.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList());
            await UpdateDashBoardCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm", purchaseOrderTransitAlarms.ToList());
            await purchaseOrderTransitAlarmsGrid.Reload();
        }

        protected async Task DisablePurchaseOrderTransitAlarm(PurchaseOrderTransitAlarm args)
        {
            var alarmVisible = purchaseAlarmsAlertVisible;
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedPurchaseOrderTransitAlarmService.AddAsync(new VisualizedPurchaseOrderTransitAlarm { PurchaseOrderTransitAlarmId = args.PurchaseOrderTransitAlarmId, EmployeeId = employee.EmployeeId, VisualizedDate = System.DateTime.Now });
                await UpdatePurchaseOrderTransitAlarmsAsync();
                purchaseAlarmsAlertVisible = alarmVisible;
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
            var originalData = await GetDashBoardCache<PurchaseOrder>("PurchaseOrder");

            purchaseOrderExpirations = await DashBoardService.GetPurchaseOrderExpirationsAsync(Settings.Value.PurchaseOrderWhiteFlag, ct);
            expiredPurchasesAlertVisible = !await IsEqual<PurchaseOrder>(purchaseOrderExpirations.OrderBy(o => o.PurchaseOrderId).ToList(), originalData.OrderBy(o => o.PurchaseOrderId).ToList());
            await UpdateDashBoardCache<PurchaseOrder>("PurchaseOrder", purchaseOrderExpirations.ToList());
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