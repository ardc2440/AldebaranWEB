using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class OutOfStockNotifications
    {
        #region Injections       

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        private IMemoryCache MemoryCache { get; set; }

        private static MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected ICacheHelper CacheHelper { get; set; }

        #endregion

        #region properties
        [Parameter]
        public int PendingStatusOrderId { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool outOfStockAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("OutOfStock-GridTimer");
        List<DataTimer> Timers;

        protected List<OutOfStockArticle> outOfStockArticles = new List<OutOfStockArticle>();
        protected LocalizedDataGrid<OutOfStockArticle> outOfStockArticlesGrid;

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

        private async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                var detailInTransit = await DashBoardService.GetTransitDetailOrdersAsync(PendingStatusOrderId);
                await UpdateItemsOutOfStockAsync(detailInTransit.ToList(), (await DashBoardService.GetAllOutOfStockReferences()).ToList());
            }
            finally
            {
                isLoadingInProgress = false;
            }
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

        async Task UpdateItemsOutOfStockAsync(List<PurchaseOrderDetail> mydetailInTransit, List<ItemReference> itemReferences)
        {
            var originalData = await GetCache<OutOfStockArticle>("OutOfStockArticle");
            outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, mydetailInTransit);
            outOfStockAlertVisible = !outOfStockArticles.OrderBy(o => o.ArticleName).ToList().IsEqual<OutOfStockArticle>(originalData.OrderBy(o => o.ArticleName).ToList());
            await UpdateCache<OutOfStockArticle>("OutOfStockArticle", outOfStockArticles);
            if (outOfStockArticlesGrid != null)
                await outOfStockArticlesGrid.Reload();
        }

        private void HandleBoolChange(bool newValue)
        {
            outOfStockAlertVisible = newValue;
        }
        #endregion

    }
}
