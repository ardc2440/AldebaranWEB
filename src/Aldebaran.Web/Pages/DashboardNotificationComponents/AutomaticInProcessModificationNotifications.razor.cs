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
    public partial class AutomaticInProcessModificationNotifications
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
        public IVisualizedAutomaticCustomerInProcessModificationService VisualizedAutomaticCustomerInProcessModificationService { get; set; }

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

        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool automaticModificationsAlertVisible = false;
        protected Employee employee;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("AutomaticModifications-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        private int currentPage = 1;
        protected IList<AutomaticCustomerOrderInProcessModification> visibleItems = new List<AutomaticCustomerOrderInProcessModification>();

        protected IEnumerable<AutomaticCustomerOrderInProcessModification> modifications = new List<AutomaticCustomerOrderInProcessModification>();
        protected LocalizedDataGrid<AutomaticCustomerOrderInProcessModification> modificationsGrid;

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
                await LoadVisibleItems();
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
                        Logger.LogError(ex, "Unable to update data for Automatic Modifications");
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

        #region Data

        public async Task Update()
        {
            try
            {
                isLoadingInProgress = true;
                await GridData_Update();
                await LoadVisibleItems();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateModificationsAsync();
                await LoadVisibleItems();
            }
            finally
            {
                isLoadingInProgress = false;
            }
            StateHasChanged();
        }

        async Task UpdateModificationsAsync(CancellationToken ct = default)
        {
            if (employee == null) return;

            var originalData = await GetCache<AutomaticCustomerOrderInProcessModification>("AutomaticCustomerOrderInProcessModification");
            modifications = await DashBoardService.GetModificatedAutomaticCustomerInProcessAlarmsAsync(employee.EmployeeId, search, ct);
            await AlertVisibleChange(!modifications.OrderBy(o => o.Order_Number).ToList().IsEqual<AutomaticCustomerOrderInProcessModification>(originalData.OrderBy(o => o.Order_Number).ToList()));
            await UpdateCache<AutomaticCustomerOrderInProcessModification>("AutomaticCustomerOrderInProcessModification", modifications.ToList());
            if (modificationsGrid != null)
                await modificationsGrid.Reload();
        }

        private async Task AlertVisibleChange(bool value)
        {
            automaticModificationsAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((12, automaticModificationsAlertVisible));
        }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await modificationsGrid.GoToPage(0);
            await GridData_Update();
        }

        protected async Task ClearSearch()
        {
            search = "";
            await modificationsGrid.GoToPage(0);
            await GridData_Update();
        }

        private async Task OnPageChanged(object args)
        {
            if (args is PagerEventArgs pageArgs)
            {
                currentPage = pageArgs.PageIndex + 1;
                await LoadVisibleItems();
            }
        }

        private async Task LoadVisibleItems()
        {
            visibleItems = modifications
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        protected async Task MarkAsVisualized(AutomaticCustomerOrderInProcessModification modification)
        {
            try
            {
                isLoadingInProgress = true;

                var visualizedModification = new VisualizedAutomaticCustomerOrderInProcessModification
                {
                    Id = modification.Id,
                    ActionType = modification.ActionType,
                    Employee_Id = employee.EmployeeId,
                    Visualized_Date = DateTime.Now
                };

                await VisualizedAutomaticCustomerInProcessModificationService.AddAsync(visualizedModification);

                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Modificación marcada",
                    Severity = NotificationSeverity.Success,
                    Detail = "La modificación ha sido marcada como visualizada."
                });

                await UpdateModificationsAsync();
                await LoadVisibleItems();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error marking modification as visualized");
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Error",
                    Severity = NotificationSeverity.Error,
                    Detail = "No se pudo marcar la modificación como visualizada."
                });
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #endregion
    }

    public abstract class AutomaticInProcessModificationNotificationsBase : ComponentBase
    {
        // Clase base para facilitar la herencia si es necesario
    }
}