using Aldebaran.Application.Services.Services;
using Aldebaran.Application.Services;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Reference_Movement;
using Newtonsoft.Json;
using Aldebaran.Infraestructure.Common.Extensions;
using DocumentFormat.OpenXml.Vml.Office;

namespace Aldebaran.Web.Pages.DashboardNotificationComponents
{
    public partial class LocalWarehouseNotifications
    {
        #region Injections

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        private IMemoryCache MemoryCache { get; set; }

        private static readonly MemoryCacheEntryOptions _cacheEntryOptions = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromDays(1) };

        [Inject]
        public IPurchaseOrderService PurchaseOrderService { get; set; }
        [Inject]
        public IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }
        [Inject]
        public IWarehouseTransferService WarehouseTransferService { get; set; }
        [Inject]
        public IWarehouseTransferDetailService WarehouseTransferDetailService { get; set; }
        [Inject]
        public ICustomerOrderService CustomerOrderService { get; set; }
        [Inject]
        public ICustomerOrderDetailService CustomerOrderDetailService { get; set; }
        [Inject]
        public IDashBoardService DashBoardService { get; set; }
        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }
        [Inject]
        protected IVisualizedLocalWarehouseAlarmService VisualizedLocalWarehouseAlarmService { get; set; }
        [Inject]
        protected ILogger<Index> Logger { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }
        #endregion

        #region Parameters
        [Parameter] public EventCallback<(int, bool)> OnAlertVisibleChanged { get; set; }

        #endregion

        #region Variables
        protected bool isLoadingInProgress;
        protected bool localWarehouseAlertVisible = false;
        protected int pageSize = 10;
        readonly GridTimer GridTimer = new GridTimer("LocalWarehouse-GridTimer");
        List<DataTimer> Timers;
        protected string search = "";
        protected Employee employee;

        private int currentPage = 1;
        protected IList<Models.ViewModels.LocalWarehouseAlarm> selectedAlarms = new List<Models.ViewModels.LocalWarehouseAlarm>();
        protected IList<Models.ViewModels.LocalWarehouseAlarm> visibleItems = new List<Models.ViewModels.LocalWarehouseAlarm>();

        protected IEnumerable<Models.ViewModels.LocalWarehouseAlarm> localWarehouseAlarms = new List<Models.ViewModels.LocalWarehouseAlarm>();
        protected LocalizedDataGrid<Models.ViewModels.LocalWarehouseAlarm> localWarehouseAlarmsGrid;

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
                selectedAlarms = new List<Models.ViewModels.LocalWarehouseAlarm>();
                GridTimer.LastUpdate = DateTime.Now;
                Console.WriteLine($"{GridTimer.LastUpdate}");
                await UpdateLocalWarehouseAlarmsAsync();
                selectedAlarms = new List<Models.ViewModels.LocalWarehouseAlarm>();
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

            await localWarehouseAlarmsGrid.GoToPage(0);

            await GridData_Update();
        }

        async Task UpdateLocalWarehouseAlarmsAsync(CancellationToken ct = default)
        {
            var originalData = await GetCache<Models.ViewModels.LocalWarehouseAlarm>("Models.ViewModels.LocalWarehouseAlarm");
            localWarehouseAlarms = await GetLocalWarehouseAlarm(ct);
            await AlertVisibleChange(!localWarehouseAlarms.OrderBy(o => o.WarehouseAlarm.AlarmDate).ToList().IsEqual<Models.ViewModels.LocalWarehouseAlarm>(originalData.OrderBy(o => o.WarehouseAlarm.AlarmDate).ToList()));
            await UpdateCache<Models.ViewModels.LocalWarehouseAlarm>("Models.ViewModels.LocalWarehouseAlarm", localWarehouseAlarms.ToList());
            if (localWarehouseAlarmsGrid != null)
                await localWarehouseAlarmsGrid.Reload();
        }


        async Task<IEnumerable<Models.ViewModels.LocalWarehouseAlarm>> GetLocalWarehouseAlarm(CancellationToken ct = default)
        {
            var alarmList = new List<Models.ViewModels.LocalWarehouseAlarm>();

            var localWarehouseAlarmList = await DashBoardService.GetLocalWarehouseAlarm(employee.EmployeeId, search, ct);

            foreach (var item in localWarehouseAlarmList)
            {
                var entity = new Models.ViewModels.LocalWarehouseAlarm { WarehouseAlarm = item };

                entity.AlarmReferences = JsonConvert.DeserializeObject<ICollection<Models.ViewModels.AlarmReference>>(entity.WarehouseAlarm.ReferenceList);
                entity.AlarmCustomerOrders = JsonConvert.DeserializeObject<ICollection<Models.ViewModels.AlarmOrder>>(entity.WarehouseAlarm.CustomerOrderList);
                switch (entity.WarehouseAlarm.DocumentType.DocumentTypeCode)
                {
                    case "O":
                        entity.PurchaseOrder = await PurchaseOrderService.FindAsync(entity.WarehouseAlarm.DocumentNumber, ct);
                        entity.PurchaseOrder.PurchaseOrderDetails = (await PurchaseOrderDetailService.GetByPurchaseOrderIdAsync(entity.PurchaseOrder.PurchaseOrderId, ct)).ToList();
                        break;
                    case "B":
                        entity.WarehouseTransfer = await WarehouseTransferService.FindAsync(entity.WarehouseAlarm.DocumentNumber, ct);
                        entity.WarehouseTransfer.WarehouseTransferDetails = (await WarehouseTransferDetailService.GetByWarehouseTransferIdAsync(entity.WarehouseTransfer.WarehouseTransferId, ct)).ToList();
                        break;
                    default:
                        throw new InvalidDataException($"El tipo de documento {entity.WarehouseAlarm.DocumentType.DocumentTypeCode} no se encuentra habilitado para esta notificación.");
                }

                foreach (var alarmOrder in entity.AlarmCustomerOrders)
                {
                    entity.CustomerOrders.Add(await CustomerOrderService.FindAsync(alarmOrder.AlarmOrderId, ct));
                }

                alarmList.Add(entity);
            }

            return alarmList;
        }

        public async Task CustomerOrderDetailInfo(int customerOrderId)
        {
            var reasonResult = await DialogService.OpenAsync<CustomerOrderPages.CustomerOrderDetails>("Detalles del pedido", new Dictionary<string, object> { { "CustomerOrderId", customerOrderId } }, options: new DialogOptions { CloseDialogOnOverlayClick = false, Width = "800px" });
            if (reasonResult == null)
                return;
        }

        private async Task AlertVisibleChange(bool value)
        {
            localWarehouseAlertVisible = value;
            await OnAlertVisibleChanged.InvokeAsync((10, localWarehouseAlertVisible));
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

        protected async Task DisableAlarm(Models.ViewModels.LocalWarehouseAlarm args)
        {
            var alertVisible = localWarehouseAlertVisible;

            if (await DialogService.Confirm("Desea ocultar las alarmas seleccionadas?. No volverán a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Ocultar alarmas") == false)
                return;

            try
            {
                if (!selectedAlarms.Any())
                {
                    await VisualizedLocalWarehouseAlarmService.AddAsync(new VisualizedLocalWarehouseAlarm { LocalWarehouseAlarmId = args.WarehouseAlarm.LocalWarehouseAlarmId, EmployeeId = employee.EmployeeId });
                    return;
                }

                isLoadingInProgress = true;
                foreach (var alarm in selectedAlarms)
                    await VisualizedLocalWarehouseAlarmService.AddAsync(new VisualizedLocalWarehouseAlarm { LocalWarehouseAlarmId = alarm.WarehouseAlarm.LocalWarehouseAlarmId, EmployeeId = employee.EmployeeId });
            }
            finally
            {
                await UpdateLocalWarehouseAlarmsAsync();
                await AlertVisibleChange(alertVisible);
                selectedAlarms = new List<Models.ViewModels.LocalWarehouseAlarm>();
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
            visibleItems = localWarehouseAlarms
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

        private async Task ToggleSelection(Models.ViewModels.LocalWarehouseAlarm item, bool isSelected)
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

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        #endregion
    }
}
