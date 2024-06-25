using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class PurchaseOrderTransitAlarmNotifications
    {
        #region Injections       

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        public IPurchaseOrderNotificationService PurchaseOrderNotificationService { get; set; }

        [Inject]
        public IVisualizedPurchaseOrderTransitAlarmService VisualizedPurchaseOrderTransitAlarmService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        private IMemoryCache MemoryCache { get; set; }

        private static MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        protected ICacheHelper CacheHelper { get; set; }

        #endregion

        #region properties

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool purchaseAlarmsAlertVisible = false;
        protected int pageSize = 7;
        protected Employee employee;
        readonly GridTimer GridTimer = new GridTimer("ExpiredReservations-GridTimer");
        List<DataTimer> Timers;

        protected IEnumerable<PurchaseOrderNotification> purchaseOrderNotifications = new List<PurchaseOrderNotification>();
        protected LocalizedDataGrid<PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;

        protected IEnumerable<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarms = new List<PurchaseOrderTransitAlarm>();
        protected LocalizedDataGrid<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarmsGrid;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                employee = await DashBoardService.FindByLoginUserIdAsync(Security.User.Id);
                await InitializeGridTimers();
                await GridData_Update();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }
        #endregion

        #region Events

        #region Timer

        public async Task Update()
        {
            await GridData_Update();
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdatePurchaseOrderTransitAlarmsAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
            StateHasChanged();
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
        #endregion

        #region Cache

        protected string GetCacheKey(string key)
        {
            return $"{Security.User.Id}-{key}";
        }

        public async Task<List<T>> GetCache<T>(string key) where T : class
        {
            var loggedUserCache = GetCacheKey(key);
            if (!MemoryCache.TryGetValue(loggedUserCache, out List<T> list))
            {
                MemoryCache.Set(loggedUserCache, new List<T>(), _cacheEntryOptions);
                return new List<T>();
            }
            return list ?? new List<T>();
        }

        public async Task UpdateCache<T>(string key, List<T> list) where T : class
        {
            MemoryCache.Set(GetCacheKey(key), list, _cacheEntryOptions);
        }

        #endregion

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        async Task UpdatePurchaseOrderTransitAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm");

            purchaseOrderTransitAlarms = await DashBoardService.GetAllTransitAlarmAsync(employee.EmployeeId, ct);
            purchaseAlarmsAlertVisible = !purchaseOrderTransitAlarms.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList().IsEqual<PurchaseOrderTransitAlarm>(originalData.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList());
            await UpdateCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm", purchaseOrderTransitAlarms.ToList());
            if (purchaseOrderTransitAlarmsGrid != null)
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
        
        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }

        private void HandleBoolChange(bool newValue)
        {
            purchaseAlarmsAlertVisible = newValue;
        }
        #endregion
    }
}
