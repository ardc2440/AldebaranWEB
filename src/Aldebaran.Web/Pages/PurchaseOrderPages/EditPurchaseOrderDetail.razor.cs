using Aldebaran.Application.Services;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class EditPurchaseOrderDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public IEnumerable<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<ServiceModel.PurchaseOrderDetail>();
        [Parameter]
        public int PURCHASE_ORDER_DETAIL_ID { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected IEnumerable<ServiceModel.Warehouse> Warehouses;
        protected ServiceModel.PurchaseOrderDetail PurchaseOrderDetail;
        protected bool IsSubmitInProgress;
        protected InventoryQuantities InventoryQuantitiesPanel;
        protected string Error;
        protected short WarehouseId;
        protected int RequestedQuantity;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Warehouses = await WarehouseService.GetAsync();
            PurchaseOrderDetail = await PurchaseOrderDetailService.FindAsync(PURCHASE_ORDER_DETAIL_ID);
            WarehouseId = PurchaseOrderDetail.WarehouseId;
            RequestedQuantity = PurchaseOrderDetail.RequestedQuantity;
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                // Un detalle de orden de compra es unico por referencia y bodega
                // Validar solo si se hizo cambio de bodega
                if (WarehouseId != PurchaseOrderDetail.WarehouseId && PurchaseOrderDetails.Any(a => a.ReferenceId == PurchaseOrderDetail.ReferenceId && a.Warehouse.WarehouseId == PurchaseOrderDetail.WarehouseId))
                {
                    IsErrorVisible = true;
                    Error = "Ya existe una referencia para la misma bodega adicionada a esta orden de compra";
                    return;
                }
                PurchaseOrderDetail.Warehouse = Warehouses.Single(s => s.WarehouseId == PurchaseOrderDetail.WarehouseId);
                DialogService.Close(PurchaseOrderDetail);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
                Restore();
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            Restore();
            DialogService.Close(null);
        }
        void Restore()
        {
            PurchaseOrderDetail.WarehouseId = WarehouseId;
            PurchaseOrderDetail.RequestedQuantity = RequestedQuantity;
        }
        #endregion
    }
}