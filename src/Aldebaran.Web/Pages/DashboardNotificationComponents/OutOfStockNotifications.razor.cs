using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement;
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

        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        public IDashBoardService DashBoardService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public bool IsModal { get; set; } = false;

        [Parameter]
        public int PendingStatusOrderId { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool outOfStockAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("OutOfStock-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        protected IEnumerable<OutOfStockArticle> outOfStockArticles = new List<OutOfStockArticle>();
        protected LocalizedDataGrid<OutOfStockArticle> outOfStockArticlesGrid;
        protected Employee employee;
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
                await UpdateItemsOutOfStockAsync();
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

            await outOfStockArticlesGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateItemsOutOfStockAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<OutOfStockArticle>("OutOfStockArticle");

            outOfStockArticles = await DashBoardService.GetOutOfStockAlarmsAsync(employee.EmployeeId, search, ct);

            outOfStockAlertVisible = !outOfStockArticles.OrderBy(o => o.ArticleName).ToList().IsEqual<OutOfStockArticle>(originalData.OrderBy(o => o.ArticleName).ToList());
            await UpdateCache<OutOfStockArticle>("OutOfStockArticle", outOfStockArticles.ToList());
            if (outOfStockArticlesGrid != null)
                await outOfStockArticlesGrid.Reload();
        }

        private void HandleBoolChange(bool newValue)
        {
            outOfStockAlertVisible = newValue;
        }

        async Task ReferenceMovementReport(int referenceId)
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReport>("Reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "ReferenceId", referenceId }, { "IsModal", true } }, options: new DialogOptions { Width = "80%", ContentCssClass = "pt-0" });
            if (result == null)
                return;
        }
        #endregion

    }
}
