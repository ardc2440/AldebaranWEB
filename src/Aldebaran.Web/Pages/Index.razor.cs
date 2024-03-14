using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages
{
    public partial class Index
    {
        #region Injections

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

        #endregion

        #region Parameters

        #endregion

        #region Global Variables

        protected IEnumerable<MinimumQuantityArticle> minimumQuantityArticles = new List<MinimumQuantityArticle>();
        protected LocalizedDataGrid<MinimumQuantityArticle> minimumQuantityArticlesGrid;
        protected IEnumerable<OutOfStockArticle> outOfStockArticles = new List<OutOfStockArticle>();
        protected LocalizedDataGrid<OutOfStockArticle> outOfStockArticlesGrid;
        protected IEnumerable<CustomerReservation> expiredReservations = new List<CustomerReservation>();
        protected LocalizedDataGrid<CustomerReservation> expiredReservationsGrid;
        protected IEnumerable<Models.ViewModels.Alarm> alarms = new List<Models.ViewModels.Alarm>();
        protected LocalizedDataGrid<Models.ViewModels.Alarm> alarmsGrid;

        protected int pageSize = 7;
        protected Employee employee;

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
            await RefreshData();
        }
        async Task RefreshData()
        {
            var detailInTransit = await GetDetailInTransitAsync();

            await RefreshMinimumQuantitiesAsync(detailInTransit);
            await RefreshItemsOutOfStokAsync(detailInTransit);
            await RefreshExpiredReservationsAsync();
            await RefreshAlarms();

            await ReloadGrids();
            StateHasChanged();
        }

        private async Task ReloadGrids()
        {
            if (minimumQuantityArticlesGrid != null)
                await minimumQuantityArticlesGrid.Reload();
            if (outOfStockArticlesGrid != null)
                await outOfStockArticlesGrid.Reload();
            if (expiredReservationsGrid != null)
                await expiredReservationsGrid.Reload();
            if (alarmsGrid != null)
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

        async Task RefreshAlarms()
        {
            var alarmList = await AlarmService.GetByEmployeeIdAsync(employee.EmployeeId);
            alarms = await new Models.ViewModels.Alarm().GetAlarmsListAsync(alarmList, AlarmService);
        }

        async Task RefreshItemsOutOfStokAsync(IEnumerable<PurchaseOrderDetail> detailInTransit)
        {
            var itemReferences = await ItemReferenceService.GetAllReferencesOutOfStock();

            outOfStockArticles = await OutOfStockArticle.GetOutOfStockArticleListAsync(itemReferences, detailInTransit);
        }

        async Task RefreshMinimumQuantitiesAsync(IEnumerable<PurchaseOrderDetail> detailInTransit)
        {
            var itemReferences = await ItemReferenceService.GetAllReferencesWithMinimumQuantity();

            minimumQuantityArticles = await MinimumQuantityArticle.GetMinimuQuantityArticleListAsync(itemReferences, detailInTransit);
        }

        async Task<IEnumerable<PurchaseOrderDetail>> GetDetailInTransitAsync()
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            var statusOrder = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            return await PurchaseOrderDetailService.GetTransitDetailOrdersAsync(statusOrder.StatusDocumentTypeId);
        }

        async Task RefreshExpiredReservationsAsync()
        {
            expiredReservations = await CustomerReservationService.GetExpiredReservationsAsync();
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
                await RefreshData();
            }
        }

        #endregion
    }
}