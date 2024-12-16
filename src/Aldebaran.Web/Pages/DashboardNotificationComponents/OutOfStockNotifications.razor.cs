using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
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

        [Inject]
        protected IVisualizedOutOfStockInventoryAlarmService VisualizedOutOfStockInventoryAlarmService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }
        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool outOfStockAlertVisible = false;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("OutOfStock-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";

        private int currentPage = 1;
        protected IList<OutOfStockArticle> selectedAlarms = new List<OutOfStockArticle>();
        protected IList<OutOfStockArticle> visibleItems = new List<OutOfStockArticle>();
        
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
                selectedAlarms = new List<OutOfStockArticle>();
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateItemsOutOfStockAsync();
                selectedAlarms = new List<OutOfStockArticle>();
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

            await outOfStockArticlesGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateItemsOutOfStockAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<OutOfStockArticle>("OutOfStockArticle");

            outOfStockArticles = await DashBoardService.GetOutOfStockAlarmsAsync(employee.EmployeeId, search, ct);

            await AlertVisibleChange(!outOfStockArticles.OrderBy(o => o.ArticleName).ToList().IsEqual<OutOfStockArticle>(originalData.OrderBy(o => o.ArticleName).ToList()));
            await UpdateCache<OutOfStockArticle>("OutOfStockArticle", outOfStockArticles.ToList());
            if (outOfStockArticlesGrid != null)
                await outOfStockArticlesGrid.Reload();
        }

        private async Task AlertVisibleChange(bool value)
        {
            outOfStockAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((2, outOfStockAlertVisible));
        }

        async Task ReferenceMovementReport(int referenceId)
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReport>("Reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "ReferenceId", referenceId }, { "IsModal", true } }, options: new DialogOptions { Width = "80%", ContentCssClass = "pt-0" });
            if (result == null)
                return;
        }

        async Task ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, content, options);

            await Task.Delay(1000);

            TooltipService.Close();
        }
        protected async Task DisableAlarm(OutOfStockArticle args)
        {
            var alertVisible = outOfStockAlertVisible;
            if (await DialogService.Confirm("Desea ocultar las alarmas seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarmas") == false)
                return;

            try
            {
                if (!selectedAlarms.Any())
                {
                    await VisualizedOutOfStockInventoryAlarmService.AddAsync(new VisualizedOutOfStockInventoryAlarm { OutOfStockInventoryAlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                    return;
                }

                isLoadingInProgress = true;
                foreach (var alarm in selectedAlarms)
                    await VisualizedOutOfStockInventoryAlarmService.AddAsync(new VisualizedOutOfStockInventoryAlarm { OutOfStockInventoryAlarmId = alarm.AlarmId, EmployeeId = employee.EmployeeId });
            }
            finally
            {
                await UpdateItemsOutOfStockAsync();
                await AlertVisibleChange(alertVisible);
                selectedAlarms = new List<OutOfStockArticle>();
                await LoadVisibleItems();
                isLoadingInProgress = false;
            }            
        }

        private async Task ShowImageDialogAsync(string articleName) => await DialogService.OpenAsync<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });

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
            visibleItems = outOfStockArticles
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

        private async Task ToggleSelection(OutOfStockArticle item, bool isSelected)
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
