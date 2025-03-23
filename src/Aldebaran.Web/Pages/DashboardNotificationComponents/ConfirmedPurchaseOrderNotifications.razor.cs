using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Infraestructure.Common.Extensions;
using Aldebaran.Web.Pages.CustomerOrderPages;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using Radzen;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class ConfirmedPurchaseOrderNotifications
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
        protected IVisualizedAutomaticInProcessAlarmService VisualizedAutomaticInProcessAlarmService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool automaticInProcessAlertVisible = false;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("AutomaticInProcess-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";
        protected Employee employee;

        private int currentPage = 1;
        protected IList<ConfirmedPurchaseOrder> selectedAlarms = new List<ConfirmedPurchaseOrder>();
        protected IList<ConfirmedPurchaseOrder> visibleItems = new List<ConfirmedPurchaseOrder>();

        protected IEnumerable<ConfirmedPurchaseOrder> confirmedPurchaseOrders = new List<ConfirmedPurchaseOrder>();
        protected IEnumerable<AutomaticCustomerOrder> automaticCustomerOrders = new List<AutomaticCustomerOrder>();
        protected IEnumerable<AutomaticCustomerOrderDetail> automaticCustomerOrderDetails = new List<AutomaticCustomerOrderDetail>();
        
        protected LocalizedDataGrid<ConfirmedPurchaseOrder> confirmedPurchaseOrdersGrid;
        protected LocalizedDataGrid<AutomaticCustomerOrder> automaticCustomerOrdersGrid;
        protected LocalizedDataGrid<AutomaticCustomerOrderDetail> automaticCustomerOrderDetailsGrid;

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
                selectedAlarms = new List<ConfirmedPurchaseOrder>();
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateConfirmedPurchaseOrdersAsync();
                selectedAlarms = new List<ConfirmedPurchaseOrder>();
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

            await confirmedPurchaseOrdersGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateConfirmedPurchaseOrdersAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<ConfirmedPurchaseOrder>("ConfirmedPurchaseOrder");
            confirmedPurchaseOrders = await DashBoardService.GetConfirmedPurchasesWithAutomaticAssigment(employee.EmployeeId, search, ct);
            await AlertVisibleChange(!confirmedPurchaseOrders.OrderBy(o => o.OrderNumber).ToList().IsEqual<ConfirmedPurchaseOrder>(originalData.OrderBy(o => o.OrderNumber).ToList()));
            await UpdateCache<ConfirmedPurchaseOrder>("ConfirmedPurchaseOrder", confirmedPurchaseOrders.ToList());
            if (confirmedPurchaseOrdersGrid != null)
                await confirmedPurchaseOrdersGrid.Reload();
        }

        private async Task AlertVisibleChange(bool value)
        {
            automaticInProcessAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((11, automaticInProcessAlertVisible));
        }

        async Task ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null)
        {
            TooltipService.Open(elementReference, content, options);

            await Task.Delay(1000);

            TooltipService.Close();
        }

        protected async Task DisableAlarm(ConfirmedPurchaseOrder args)
        {
            var alertVisible = automaticInProcessAlertVisible;

            if (await DialogService.Confirm("Desea ocultar las alarmas seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarmas") == false)
                return;

            try
            {
                if (!selectedAlarms.Any())
                {
                    await VisualizedAutomaticInProcessAlarmService.AddAsync(new VisualizedAutomaticInProcess { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                    return;
                }

                isLoadingInProgress = true;
                foreach (var alarm in selectedAlarms)
                    await VisualizedAutomaticInProcessAlarmService.AddAsync(new VisualizedAutomaticInProcess { AlarmId = alarm.AlarmId, EmployeeId = employee.EmployeeId });
            }
            finally
            {
                await UpdateConfirmedPurchaseOrdersAsync();
                await AlertVisibleChange(alertVisible);
                selectedAlarms = new List<ConfirmedPurchaseOrder>();
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
            visibleItems = confirmedPurchaseOrders
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

        private async Task ToggleSelection(ConfirmedPurchaseOrder item, bool isSelected)
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

        protected async Task GetCustomerOrders(ConfirmedPurchaseOrder args)
        {
            var automaticOrdersResult = await DashBoardService.GetAutomaticCustomerOrdersAssigment(args.AlarmId);

            if (automaticOrdersResult != null)
            {
                automaticCustomerOrders = automaticOrdersResult.ToList();
            }
        }

        protected async Task GetCustomerOrderDetails(AutomaticCustomerOrder args)
        {
            var automaticOrderDetailsResult = await DashBoardService.GetAutomaticCustomerOrderDetailsAssigment(args.OrderId);

            if (automaticOrderDetailsResult != null)
            {
                automaticCustomerOrderDetails = automaticOrderDetailsResult.ToList();
            }
        }

        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }

        async Task ReferenceMovementReport(int referenceId)
        {
            var result = await DialogService.OpenAsync<ReferenceMovementReport>("Reporte de movimientos de artículos", parameters: new Dictionary<string, object> { { "ReferenceId", referenceId }, { "IsModal", true } }, options: new DialogOptions { Width = "80%", ContentCssClass = "pt-0" });
            if (result == null)
                return;
        }

        public async Task PurchaseOrderDetailInfo(string documentType, int purchaseOrderId)        
        {
            if (documentType == "O")
            {
                var reasonResult = await DialogService.OpenAsync<PurchaseOrderPages.PurchaseOrderDetails>("Detalles de la orden de compra", new Dictionary<string, object> { { "PurchaseOrderId", purchaseOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
                if (reasonResult == null)
                    return;
            }
        }

        protected async Task DownloadAsync(MouseEventArgs arg, int customerOrderId)
        {
            await DialogService.OpenAsync<CustomerOrderSummary>(null, new Dictionary<string, object> { { "Id", customerOrderId }, { "NotificationTemplateName", "N/A" }, {"ShowMailDialog", false} }, options: new DialogOptions { ShowTitle = false, ShowClose = true, CloseDialogOnEsc = true, CloseDialogOnOverlayClick = true, Width = "800px" });            
        }

        #endregion
    }
}
