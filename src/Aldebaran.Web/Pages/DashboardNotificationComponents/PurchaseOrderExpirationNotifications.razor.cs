using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.Extensions.Options;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using Aldebaran.Web.Models;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class PurchaseOrderExpirationNotifications
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
        public IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }
       
        [Inject]
        public IOptions<AppSettings> Settings { get; set; }

        [Inject]
        private IMemoryCache MemoryCache { get; set; }

        private static MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        protected ICacheHelper CacheHelper { get; set; }

        #endregion

        #region properties
        [Parameter]
        public int PendingStatusOrderId { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool expiredPurchasesAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("ExpiredPurchaseOrder-GridTimer");
        List<DataTimer> Timers;

        protected IEnumerable<PurchaseOrder> purchaseOrderExpirations = new List<PurchaseOrder>();
        protected LocalizedDataGrid<PurchaseOrder> purchaseOrderExpirationsGrid;
        protected IEnumerable<CustomerOrderAffectedByPurchaseOrderUpdate> customerOrdersAffected = new List<CustomerOrderAffectedByPurchaseOrderUpdate>();
        protected LocalizedDataGrid<CustomerOrderAffectedByPurchaseOrderUpdate> customerOrdersAffectedGrid;

        #endregion

        #region Override
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Timers = TimerPreferenceService.Timers;
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
                await UpdatePurchaseOrderExpirationsAsync();
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
                
        public async Task UpdatePurchaseOrderExpirationsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<PurchaseOrder>("PurchaseOrder");

            purchaseOrderExpirations = await DashBoardService.GetPurchaseOrderExpirationsAsync(Settings.Value.PurchaseOrderWhiteFlag, ct);
            expiredPurchasesAlertVisible = !purchaseOrderExpirations.OrderBy(o => o.PurchaseOrderId).ToList().IsEqual<PurchaseOrder>(originalData.OrderBy(o => o.PurchaseOrderId).ToList());
            await UpdateCache<PurchaseOrder>("PurchaseOrder", purchaseOrderExpirations.ToList());
            if (purchaseOrderExpirationsGrid != null)
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
                args.Attributes.Add("style", $"background-color:#ffa7a7");
            }
            else
            {
                if (days <= Settings.Value.PurchaseOrderYellowFlag)
                {
                    args.Attributes.Add("style", $"background-color:#ffff58");
                }
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
            expiredPurchasesAlertVisible = newValue;
        }
        #endregion
    }
}
