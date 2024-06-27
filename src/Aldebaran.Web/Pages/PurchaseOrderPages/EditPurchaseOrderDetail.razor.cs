using Aldebaran.Application.Services;
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
        protected ILogger<EditPurchaseOrderDetail> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IWarehouseService WarehouseService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public IEnumerable<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<ServiceModel.PurchaseOrderDetail>();
        [Parameter]
        public int? PURCHASE_ORDER_DETAIL_ID { get; set; }
        [Parameter]
        public ServiceModel.PurchaseOrderDetail PurchaseOrderDetail { get; set; }
        [Parameter]
        public int ProviderId { get; set; }
        [Parameter]
        public int PurchaseOrderId { get; set; } = -1;
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected IEnumerable<ServiceModel.Warehouse> Warehouses;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected short WarehouseId;
        protected int RequestedQuantity;
        protected ServiceModel.ItemReference ItemReference;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Warehouses = await WarehouseService.GetAsync();
                if (PURCHASE_ORDER_DETAIL_ID == null && PurchaseOrderDetail == null)
                    throw new ArgumentNullException($"{nameof(PURCHASE_ORDER_DETAIL_ID)} y {nameof(PurchaseOrderDetail)} no pueden ser nulos.");
                if (PURCHASE_ORDER_DETAIL_ID != null)
                    PurchaseOrderDetail = await PurchaseOrderDetailService.FindAsync(PURCHASE_ORDER_DETAIL_ID.Value);
                WarehouseId = PurchaseOrderDetail.WarehouseId;
                RequestedQuantity = PurchaseOrderDetail.RequestedQuantity;
                ItemReference = await ItemReferenceService.FindAsync(PurchaseOrderDetail.ReferenceId);
            }
            finally
            {
                isLoadingInProgress = false;
            }

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

                if (!await PurchaseOrderDetailService.IsValidPurchaseOrderVariation(ProviderId, PurchaseOrderDetail.ReferenceId, PurchaseOrderId))
                    if (await DialogService.Confirm("Ha ingresado una cantidad fuera del rango promedio de ordenes de compra de la referencia " +
                            "con este proveedor. Desea continuar con el proceso?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cantidad") == false) return;

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