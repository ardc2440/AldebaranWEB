using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Aldebaran.Web.Utils;
using Radzen;
using Aldebaran.Web.Pages.CustomerOrderPages;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class MinimumQuantityNotifications
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
        protected bool minimumAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("MinimumQuantity-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        protected List<MinimumQuantityArticle> minimumQuantityArticles = new List<MinimumQuantityArticle>();
        protected LocalizedDataGrid<MinimumQuantityArticle> minimumQuantityArticlesGrid;

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

                var detailInTransit = await DashBoardService.GetTransitDetailOrdersAsync(PendingStatusOrderId, search);

                await UpdateMinimumQuantitiesAsync(detailInTransit.ToList(), (await DashBoardService.GetAllReferencesWithMinimumQuantityAsync(search)).ToList());
            }
            finally
            {
                isLoadingInProgress = false;
            }
            StateHasChanged();
        }        

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await minimumQuantityArticlesGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateMinimumQuantitiesAsync(List<PurchaseOrderDetail> referencesInTransit, List<ItemReference> references)
        {
            var originalData = await GetCache<MinimumQuantityArticle>("MinimumQuantityArticle");

            minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(references, referencesInTransit);
            minimumAlertVisible = !minimumQuantityArticles.OrderBy(o => o.ArticleName).ToList().IsEqual<MinimumQuantityArticle>(originalData.OrderBy(o => o.ArticleName).ToList());
            await UpdateCache<MinimumQuantityArticle>("MinimumQuantityArticle", minimumQuantityArticles);
            if (minimumQuantityArticlesGrid != null)
                await minimumQuantityArticlesGrid.Reload();
        }

        private void HandleBoolChange(bool newValue)
        {
            minimumAlertVisible = newValue;
        }
        #endregion
    }
}
