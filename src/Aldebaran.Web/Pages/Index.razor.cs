using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
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

        [Inject]
        public IOptions<AppSettings> Settings { get; set; }

        [Inject]
        protected IMemoryCache MemoryCache { get; set; }

        #endregion

        #region Parameters

        #endregion

        #region Global Variables
                
        protected IEnumerable<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarms = new List<PurchaseOrderTransitAlarm>();
        protected LocalizedDataGrid<PurchaseOrderTransitAlarm> purchaseOrderTransitAlarmsGrid;

        protected IEnumerable<PurchaseOrderNotification> purchaseOrderNotifications = new List<PurchaseOrderNotification>();
        protected LocalizedDataGrid<PurchaseOrderNotification> PurchaseOrderNotificationsDataGrid;

        
        protected int pageSize = 7;
        protected Employee employee;
        protected DocumentType orderDocumentType;
        protected StatusDocumentType pendingStatusOrder;

        protected bool generalAlertVisible = false;

        
        protected bool purchaseAlarmsAlertVisible = false;

        List<DataTimer> Timers;
        readonly GridTimer GridTimer = new GridTimer("Dahsboard-GridTimer");
        private MemoryCacheEntryOptions _cacheEntryOptions;
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
                await InitializeGridTimers();
                employee = await DashBoardService.FindByLoginUserIdAsync(Security.User.Id);
                orderDocumentType = await DashBoardService.FindByCodeAsync("O");
                pendingStatusOrder = await DashBoardService.FindByDocumentAndOrderAsync(orderDocumentType.DocumentTypeId, 1);
                _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };
                await GridData_Update();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

        string GetCacheKey(string key)
        {
            return $"{Security.User.Id}-{key}";
        }

        public async Task UpdateDashBoardCache<T>(string key, List<T> list) where T : class
        {
            MemoryCache.Set(GetCacheKey(key), list, _cacheEntryOptions);
        }

        public async Task<List<T>> GetDashBoardCache<T>(string key) where T : class
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
            try
            {
                isLoadingInProgress = true;
                generalAlertVisible = false;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
              /*
                
                await UpdatePurchaseOrderTransitAlarmsAsync();                
              */

                if (purchaseAlarmsAlertVisible) { generalAlertVisible = true; }
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        private async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }
        
        public void Dispose()
        {
            GridTimer.Dispose();
        }

        protected async Task GeneralAlertClick()
        {
            generalAlertVisible = false;
                      
            purchaseAlarmsAlertVisible = false;
        }
        
       
        protected async Task PurchaseAlarmAlertClick()
        {
            purchaseAlarmsAlertVisible = false;
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
                

        #region ExpiredCustomerOrders

        #endregion

        #region PurchaseOrderTransitAlarms
        async Task UpdatePurchaseOrderTransitAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetDashBoardCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm");

            purchaseOrderTransitAlarms = await DashBoardService.GetAllTransitAlarmAsync(employee.EmployeeId, ct);
            purchaseAlarmsAlertVisible = !await IsEqual<PurchaseOrderTransitAlarm>(purchaseOrderTransitAlarms.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList(), originalData.OrderBy(o => o.PurchaseOrderTransitAlarmId).ToList());
            await UpdateDashBoardCache<PurchaseOrderTransitAlarm>("PurchaseOrderTransitAlarm", purchaseOrderTransitAlarms.ToList());
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
        #endregion

        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }
        #endregion
    }
}