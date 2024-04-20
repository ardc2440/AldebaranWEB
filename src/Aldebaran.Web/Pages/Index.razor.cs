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
        readonly GridTimer GridTimer = new GridTimer("Dahsboard-GridTimer");
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Timers = TimerPreferenceService.Timers;
            await InitializeGridTimers();
            employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
            await GridData_Update();
        }
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
        private async Task GridData_Update()
        {
            GridTimer.LastUpdate = DateTime.Now;
            Console.WriteLine($"{GridTimer.LastUpdate}");
            var detailInTransit = await GetDetailInTransit();
            var itemReferences = await ItemReferenceService.GetAllReferencesWithMinimumQuantityAsync();
            await UpdateMinimumQuantitiesAsync(detailInTransit.ToList(), itemReferences.ToList());
            await UpdateItemsOutOfStockAsync(detailInTransit.ToList(), itemReferences.ToList());
            await UpdateExpiredReservationsAsync();
            await UpdateUserAlarmsAsync();
        }
        async Task UpdateMinimumQuantitiesAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            minimumQuantityArticles = MinimumQuantityArticle.GetMinimuQuantityArticleList(itemReferences, detailInTransit);
            await minimumQuantityArticlesGrid.Reload();
        }
        async Task UpdateItemsOutOfStockAsync(List<PurchaseOrderDetail> detailInTransit, List<ItemReference> itemReferences)
        {
            outOfStockArticles = OutOfStockArticle.GetOutOfStockArticleList(itemReferences, detailInTransit);
            await outOfStockArticlesGrid.Reload();
        }
        async Task UpdateExpiredReservationsAsync(CancellationToken ct = default)
        {
            expiredReservations = (await CustomerReservationService.GetExpiredReservationsAsync(ct)).ToList();
            await expiredReservationsGrid.Reload();
        }
        async Task UpdateUserAlarmsAsync(CancellationToken ct = default)
        {
            var alarmList = await AlarmService.GetByEmployeeIdAsync(employee.EmployeeId, ct);
            alarms = await Models.ViewModels.Alarm.GetAlarmsListAsync(alarmList.ToList(), AlarmService);
            await alarmsGrid.Reload();
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

        async Task<IEnumerable<PurchaseOrderDetail>> GetDetailInTransit()
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            var statusOrder = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            return await PurchaseOrderDetailService.GetTransitDetailOrdersAsync(statusOrder.StatusDocumentTypeId);
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

        private async Task GridaData_UpdateOnTimerChange(object value)
        {
            var milliseconds = (double)value;
            GridTimer.UpdateTimerInterval(milliseconds);
            TimerPreferenceService.UpdateTimerPreferences(GridTimer.Key, milliseconds);
        }
        #endregion

        public void Dispose()
        {
            GridTimer.Dispose();
        }
    }
}