using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Application.Services;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;
using Aldebaran.Infraestructure.Common.Extensions;
using Microsoft.CodeAnalysis;
using Aldebaran.Application.Services.Models;

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
        protected TooltipService TooltipService { get; set; }

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

        protected int pageSize = 7;

        #endregion

        #region Overrides
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var detailInTransit = await GetDetailInTransitAsync();
            await RefreshMinimumQuantitiesAsync(detailInTransit);
            await RefreshItemsOutOfStokAsync(detailInTransit);
            await ExpiredReservationsAsync();
            //await RefreshWarehouses();
            //await RefreshTransitOrders();
            StateHasChanged();
        }

        async Task Reset()
        {

            minimumQuantityArticles = new List<MinimumQuantityArticle>();
            outOfStockArticles = new List<OutOfStockArticle>();
            expiredReservations = new List<CustomerReservation>();

            //ItemReferenceInventories = new List<ItemReferenceInventory>();
            await minimumQuantityArticlesGrid.Reload();
            await outOfStockArticlesGrid.Reload();
            await expiredReservationsGrid.Reload();
            //await ItemReferenceInventoryGrid.Reload();
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

        async Task RefreshItemsOutOfStokAsync(IEnumerable<PurchaseOrderDetail> detailInTransit)
        {
            var itemReferences = await ItemReferenceService.GetAllReferencesOutOfStock();

            outOfStockArticles = await OutOfStockArticle.GetOutOfStockArticleListAsync(itemReferences, detailInTransit);

            if (outOfStockArticlesGrid != null)
                await outOfStockArticlesGrid.Reload();
        }

        async Task RefreshMinimumQuantitiesAsync(IEnumerable<PurchaseOrderDetail> detailInTransit)
        {
            var itemReferences = await ItemReferenceService.GetAllReferencesWithMinimumQuantity();

            minimumQuantityArticles = await MinimumQuantityArticle.GetMinimuQuantityArticleListAsync(itemReferences, detailInTransit);

            if (minimumQuantityArticlesGrid != null)
                await minimumQuantityArticlesGrid.Reload();
        }

        async Task<IEnumerable<PurchaseOrderDetail>> GetDetailInTransitAsync()
        {
            var documentType = await DocumentTypeService.FindByCodeAsync("O");
            var statusOrder = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            return await PurchaseOrderDetailService.GetTransitDetailOrdersAsync(statusOrder.StatusDocumentTypeId);
        }

        async Task ExpiredReservationsAsync()
        {
            expiredReservations = await CustomerReservationService.GetExpiredReservationsAsync();

            if (expiredReservationsGrid != null)
                await expiredReservationsGrid.Reload();
        }

        protected async Task OpenCustomerReservation(CustomerReservation args)
        {
            NavigationManager.NavigateTo("send-to-customer-order/view/" + args.CustomerReservationId);
        }
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        #endregion

    }
}