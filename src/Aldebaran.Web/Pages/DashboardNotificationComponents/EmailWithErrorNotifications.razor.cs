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
    public partial class EmailWithErrorNotifications
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
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICacheHelper CacheHelper { get; set; }

        #endregion

        #region properties

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool emailNotificationsAlertVisible = false;
        protected int pageSize = 7;
        readonly GridTimer GridTimer = new GridTimer("EmailErrors-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        protected IEnumerable<NotificationWithError> notificationsWithError = new List<NotificationWithError>();
        protected LocalizedDataGrid<NotificationWithError> notificationsWithErrorGrid;

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
                await UpdateMailErrorNotificationsAsync();
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

            await notificationsWithErrorGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateMailErrorNotificationsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<NotificationWithError>("NotificationWithError");

            notificationsWithError = (await DashBoardService.GetNotificationsWithError(search, ct)).ToList();
            emailNotificationsAlertVisible = !notificationsWithError.OrderBy(o => o.NotificationDate).ToList().IsEqual<NotificationWithError>(originalData.OrderBy(o => o.NotificationDate).ToList());
            await UpdateCache<NotificationWithError>("NotificationWithError", notificationsWithError.ToList());
            if (notificationsWithErrorGrid != null)
                await notificationsWithErrorGrid.Reload();
        }
        
        private void HandleBoolChange(bool newValue)
        {
            emailNotificationsAlertVisible = newValue;
        }
        #endregion
    }
}
