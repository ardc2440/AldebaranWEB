using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Utils;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages
{
    public partial class Index : IDisposable
    {
        #region Injections
        [Inject]
        protected ILogger<Index> Logger { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        public IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        public IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        public IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        public IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        public ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        public IAlarmService AlarmService { get; set; }

        [Inject]
        public IVisualizedAlarmService VisualizedAlarmService { get; set; }

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        public ITimerPreferenceService TimerPreferenceService { get; set; }

        #endregion

        #region Parameters

        #endregion

        #region Global Variables

        protected List<MinimumQuantityArticle> minimumQuantityArticles = new List<MinimumQuantityArticle>();
        protected LocalizedDataGrid<MinimumQuantityArticle> minimumQuantityArticlesGrid;
        protected List<OutOfStockArticle> outOfStockArticles = new List<OutOfStockArticle>();
        protected LocalizedDataGrid<OutOfStockArticle> outOfStockArticlesGrid;
        protected List<CustomerReservation> expiredReservations = new List<CustomerReservation>();
        protected LocalizedDataGrid<CustomerReservation> expiredReservationsGrid;
        protected List<Models.ViewModels.Alarm> alarms = new List<Models.ViewModels.Alarm>();
        protected LocalizedDataGrid<Models.ViewModels.Alarm> alarmsGrid;

        protected int pageSize = 7;
        protected Employee employee;
        List<DataTimer> Timers;
        readonly GridTimer MinimumQuantityGridTimer = new GridTimer("MinimumQuantityGridTimer");
        readonly GridTimer ItemsOutOfStockGridTimer = new GridTimer("ItemsOutOfStockGridTimer");
        readonly GridTimer ExpiredReservationsGridTimer = new GridTimer("ExpiredReservationsGridTimer");
        readonly GridTimer UserAlarmsGridTimer = new GridTimer("UserAlarmsGridTimer");
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Timers = TimerPreferenceService.Timers;
            await Task.Yield();
            await InitializeGridTimers();
            employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
            await UpdateMinimumQuantitiesAsync();
            await UpdateItemsOutOfStockAsync();
            await UpdateExpiredReservationsAsync();
            await UpdateUserAlarmsAsync();
        }
        async Task InitializeGridTimers()
        {
            await MinimumQuantityGridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(MinimumQuantityGridTimer.Key), async (sender, e) =>
            {
                await UpdateMinimumQuantitiesAsync();
            });

            await ItemsOutOfStockGridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(ItemsOutOfStockGridTimer.Key), async (sender, e) =>
            {
                await UpdateItemsOutOfStockAsync();
            });

            await ExpiredReservationsGridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(ExpiredReservationsGridTimer.Key), async (sender, e) =>
            {
                await UpdateExpiredReservationsAsync();
            });

            await UserAlarmsGridTimer.InitializeTimer(TimerPreferenceService.GetTimerPreferences(UserAlarmsGridTimer.Key), async (sender, e) =>
            {
                await UpdateUserAlarmsAsync();
            });
        }
        async Task UpdateMinimumQuantitiesAsync()
        {
            await InvokeAsync(async () =>
            {
                await Task.Yield();
                MinimumQuantityGridTimer.IsLoading = true;
                try
                {
                    StateHasChanged();
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    var detailInTransit = GetDetailInTransitAsync();
                    var itemReferences = ItemReferenceService.GetAllReferencesWithMinimumQuantity();
                    minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(itemReferences, detailInTransit);
                    await minimumQuantityArticlesGrid.Reload();
                    Console.WriteLine($"=> {MinimumQuantityGridTimer.Key}: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to update data for MinimumQuantities");
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Cantidades mínimas",
                        Severity = NotificationSeverity.Success,
                        Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                    });
                }
                finally
                {
                    MinimumQuantityGridTimer.IsLoading = false;
                    StateHasChanged();
                }
            });
        }
        async Task UpdateItemsOutOfStockAsync()
        {
            await InvokeAsync(async () =>
            {
                await Task.Yield();
                ItemsOutOfStockGridTimer.IsLoading = true;
                try
                {
                    var detailInTransit = GetDetailInTransitAsync();
                    var itemReferences = ItemReferenceService.GetAllReferencesOutOfStock();
                    outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, detailInTransit);
                    await outOfStockArticlesGrid.Reload();
                    Console.WriteLine($"==> {ItemsOutOfStockGridTimer.Key}: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to update data for ItemsOutOfStock");
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Artículos sin disponible",
                        Severity = NotificationSeverity.Success,
                        Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                    });
                }
                finally
                {
                    ItemsOutOfStockGridTimer.IsLoading = false;
                    StateHasChanged();
                }
            });
        }
        async Task UpdateExpiredReservationsAsync()
        {
            await InvokeAsync(async () =>
            {
                await Task.Yield();
                ExpiredReservationsGridTimer.IsLoading = true;
                try
                {
                    expiredReservations = CustomerReservationService.GetExpiredReservations();
                    await expiredReservationsGrid.Reload();
                    Console.WriteLine($"===> {ExpiredReservationsGridTimer.Key}: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to update data for ExpiredReservations");
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Reservas vencidas",
                        Severity = NotificationSeverity.Success,
                        Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                    });
                }
                finally
                {
                    ExpiredReservationsGridTimer.IsLoading = false;
                    StateHasChanged();
                }
            });
        }
        async Task UpdateUserAlarmsAsync()
        {
            await InvokeAsync(async () =>
            {
                await Task.Yield();
                UserAlarmsGridTimer.IsLoading = true;
                try
                {
                    var alarmList = (await AlarmService.GetByEmployeeIdAsync(employee.EmployeeId)).ToList();
                    alarms = Models.ViewModels.Alarm.GetAlarmsList(alarmList, AlarmService);
                    await alarmsGrid.Reload();
                    Console.WriteLine($"====> {UserAlarmsGridTimer.Key}: {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unable to update data for UserAlarms");
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Alarmas del día",
                        Severity = NotificationSeverity.Success,
                        Detail = $"No se ha podido actualizar la información, favor intente manualmente."
                    });
                }
                finally
                {
                    UserAlarmsGridTimer.IsLoading = false;
                    StateHasChanged();
                }
            });
        }

        #endregion

        #region Events
        void OnResize(RadzenSplitterResizeEventArgs args)
        {
            if (args.Pane != null)
            {
                var newSize = args.NewSize;
                var rowSize = 4.1641;
                var newRowSize = Convert.ToInt32(newSize / rowSize) - 5; //Se quitan las 2 rows de la paginacion
                newRowSize = newRowSize < 1 ? 1 : newRowSize;
                pageSize = newRowSize;
            }
        }

        List<PurchaseOrderDetail> GetDetailInTransitAsync()
        {
            var documentType = DocumentTypeService.FindByCode("O");
            var statusOrder = StatusDocumentTypeService.FindByDocumentAndOrder(documentType.DocumentTypeId, 1);
            return PurchaseOrderDetailService.GetTransitDetailOrders(statusOrder.StatusDocumentTypeId);
        }

        protected async Task OpenCustomerReservation(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/view/" + args.CustomerReservationId);
        }
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task DisableAlarm(Models.ViewModels.Alarm args)
        {
            if (await DialogService.Confirm("Desea marcar esta alarma como leída?. No volverá a salir en su Home", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Marcar alarma leída") == true)
            {
                await VisualizedAlarmService.AddAsync(new VisualizedAlarm { AlarmId = args.AlarmId, EmployeeId = employee.EmployeeId });
                await UpdateUserAlarmsAsync();
            }
        }

        private async Task MinimumQuantityData_UpdateOnTimerChange(object value)
        {
            await UpdateTimer((double)value, MinimumQuantityGridTimer);
        }

        private async Task ItemsOutOfStockData_UpdateOnTimerChange(object value)
        {
            await UpdateTimer((double)value, ItemsOutOfStockGridTimer);
        }

        private async Task ExpiredReservationsData_UpdateOnTimerChange(object value)
        {
            await UpdateTimer((double)value, ExpiredReservationsGridTimer);
        }

        private async Task UserAlarmsData_UpdateOnTimerChange(object value)
        {
            await UpdateTimer((double)value, UserAlarmsGridTimer);
        }

        async Task UpdateTimer(double milliseconds, GridTimer gridTimer)
        {
            gridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(gridTimer.Key, milliseconds);
        }
        #endregion

        public void Dispose()
        {
            MinimumQuantityGridTimer.Dispose();
            ItemsOutOfStockGridTimer.Dispose();
            ExpiredReservationsGridTimer.Dispose();
            UserAlarmsGridTimer.Dispose();
        }
    }
}