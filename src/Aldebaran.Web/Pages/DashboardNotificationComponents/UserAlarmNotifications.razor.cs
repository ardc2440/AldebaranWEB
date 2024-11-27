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
    public partial class UserAlarmNotifications
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
        public IAlarmService AlarmService { get; set; }

        [Inject]
        public IVisualizedAlarmService VisualizedAlarmService { get; set; }

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
        protected bool alarmsAlertVisible = false;
        protected Employee employee;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("UserAlarms-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        private int currentPage = 1;
        protected IList<Models.ViewModels.Alarm> selectedAlarms = new List<Models.ViewModels.Alarm>();
        protected IList<Models.ViewModels.Alarm> visibleItems = new List<Models.ViewModels.Alarm>();
        
        protected List<Models.ViewModels.Alarm> alarms = new List<Models.ViewModels.Alarm>();
        protected LocalizedDataGrid<Models.ViewModels.Alarm> alarmsGrid;

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
                selectedAlarms = new List<Models.ViewModels.Alarm>();
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateUserAlarmsAsync();
                selectedAlarms = new List<Models.ViewModels.Alarm>();
                await LoadVisibleItems();
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

            await alarmsGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateUserAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<Models.ViewModels.Alarm>("Alarm");

            var alarmList = await DashBoardService.GetByEmployeeIdAsync(employee.EmployeeId, ct);
            alarms = await Models.ViewModels.Alarm.GetAlarmsListAsync(alarmList.ToList(), AlarmService, ct);

            if (!string.IsNullOrEmpty(search))
                alarms = alarms.Where(w => w.AlarmMessage.Contains(search) ||
                                           w.CreationDate.ToString(SharedLocalizer["date:format"]).Contains(search) ||
                                           w.ExecutionDate.ToString(SharedLocalizer["date:format"]).Contains(search) ||
                                           w.DocumentTypeName.Contains(search) ||
                                           w.DocumentNumber.Contains(search)).ToList();

            await AlertVisibleChange(!alarms.OrderBy(o => o.AlarmId).ToList().IsEqual<Models.ViewModels.Alarm>(originalData.OrderBy(o => o.AlarmId).ToList()));
            await UpdateCache<Models.ViewModels.Alarm>("Alarm", alarms);
            if (alarmsGrid != null)
                await alarmsGrid.Reload();
        }

        protected async Task DisableAlarm(Models.ViewModels.Alarm args)
        {
            var alertVisible = alarmsAlertVisible;
            
            if (await DialogService.Confirm("Desea ocultar las alarmas seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarmas") == false)
                return;
                        
            try
            {
                if (!selectedAlarms.Any())
                {
                    await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                    return;
                }

                isLoadingInProgress = true;
                foreach (var alarm in selectedAlarms)
                    await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = alarm.AlarmId, EmployeeId = employee.EmployeeId });
            }
            finally
            {
                await UpdateUserAlarmsAsync();
                await AlertVisibleChange(alertVisible);
                selectedAlarms = new List<Models.ViewModels.Alarm>();
                await LoadVisibleItems();
                isLoadingInProgress = false;
            }
        }

        async Task ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, content, options);

            await Task.Delay(1000);

            TooltipService.Close();
        }

        private async Task AlertVisibleChange(bool value)
        {
            alarmsAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((3, alarmsAlertVisible));
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
            visibleItems = alarms
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        private async Task<bool> IsAllPageSelected()
        {
            return visibleItems.All(item => selectedAlarms.Contains(item)) && (selectedAlarms.Any());
        }

        private async Task SelectAllItems(bool select)
        {
            if (select)
            {
                // Añadir los ítems visibles a la lista de selección, si no están ya seleccionados
                foreach (var item in visibleItems)
                {
                    if (!selectedAlarms.Contains(item))
                    {
                        selectedAlarms.Add(item);
                    }
                }
            }
            else
            {
                // Eliminar los ítems visibles de la lista de selección
                foreach (var item in visibleItems)
                {
                    selectedAlarms.Remove(item);
                }
            }
        }

        private async Task ToggleSelection(Models.ViewModels.Alarm item, bool isSelected)
        {
            if (isSelected)
            {
                // Asegurarse de que el ítem se añada a la lista global de selección
                if (!selectedAlarms.Contains(item))
                {
                    selectedAlarms.Add(item);
                }
            }
            else
            {
                // Si se desmarca, eliminamos el ítem de la lista de selección
                selectedAlarms.Remove(item);
            }
        }
        #endregion
    }
}
