using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Aldebaran.Web.Utils;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class MinimumLocalWarehouseQuantityNotifications
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

        [Inject]
        protected IVisualizedMinimumLocalWarehouseQuantityAlarmService VisualizedMinimumLocalWarehouseQuantityAlarmService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool minimumAlertVisible = false;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("MinimumLocalWarehouseQuantity-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";
        protected Employee employee;
        protected IList<MinimumLocalWarehouseQuantityArticle> selectedAlarms;

        protected IEnumerable<MinimumLocalWarehouseQuantityArticle> minimumQuantityArticles = new List<MinimumLocalWarehouseQuantityArticle>();
        protected LocalizedDataGrid<MinimumLocalWarehouseQuantityArticle> minimumQuantityArticlesGrid;

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

        #region Events
        public async Task Update()
        {
            await GridData_Update();
        }

        private async Task GridData_Update()
        {
            try
            {
                isLoadingInProgress = true;
                selectedAlarms = new List<MinimumLocalWarehouseQuantityArticle>();
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateMinimumQuantitiesAsync();
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

        async Task UpdateMinimumQuantitiesAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<MinimumLocalWarehouseQuantityArticle>("MinimumLocalWarehouseQuantityArticle");
            minimumQuantityArticles = await DashBoardService.GetMinimumLocalWarehouseQuantityAlarmsAsync(employee.EmployeeId, search, ct);
            await AlertVisibleChange(!minimumQuantityArticles.OrderBy(o => o.ArticleName).ToList().IsEqual<MinimumLocalWarehouseQuantityArticle>(originalData.OrderBy(o => o.ArticleName).ToList()));
            await UpdateCache<MinimumLocalWarehouseQuantityArticle>("MinimumLocalWarehouseQuantityArticle", minimumQuantityArticles.ToList());
            if (minimumQuantityArticlesGrid != null)
                await minimumQuantityArticlesGrid.Reload();
        }

        private async Task AlertVisibleChange(bool value)
        {
            minimumAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((9, minimumAlertVisible));
        }

        async Task ReferenceMovementReport(int referenceId)
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReport>("Reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "ReferenceId", referenceId }, { "IsModal", true } }, options: new DialogOptions { Width = "80%", ContentCssClass = "pt-0" });
            if (result == null)
                return;
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task DisableAlarm(MinimumLocalWarehouseQuantityArticle args)
        {
            var alertVisible = minimumAlertVisible;

            if (selectedAlarms.Any())
            {
                if (await DialogService.Confirm("Desea ocultar las alarmas seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarmas") == true)
                {
                    try
                    {
                        isLoadingInProgress = true;
                        foreach (var alarm in selectedAlarms)
                            await VisualizedMinimumLocalWarehouseQuantityAlarmService.AddAsync(new VisualizedMinimumLocalWarehouseQuantityAlarm { AlarmId = alarm.AlarmId, EmployeeId = employee.EmployeeId });
                    }
                    finally
                    {
                        selectedAlarms = new List<MinimumLocalWarehouseQuantityArticle>();
                        isLoadingInProgress = false;
                    }

                    await UpdateMinimumQuantitiesAsync();
                    await AlertVisibleChange(alertVisible);
                }
            }
            else if (await DialogService.Confirm("Desea ocultar esta alarma?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarma") == true)
            {
                await VisualizedMinimumLocalWarehouseQuantityAlarmService.AddAsync(new VisualizedMinimumLocalWarehouseQuantityAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                await UpdateMinimumQuantitiesAsync();
                await AlertVisibleChange(alertVisible);
            }
        }

        private async Task ShowImageDialogAsync(string articleName) => await DialogService.OpenAsync<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });

        #endregion
    }
}
