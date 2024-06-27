using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderDetail
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
        public IEnumerable<ServiceModel.ItemReference> ProviderItemReferences { get; set; } = new List<ServiceModel.ItemReference>();
        [Parameter]
        public IEnumerable<ServiceModel.PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<ServiceModel.PurchaseOrderDetail>();
        [Parameter]
        public int LastReferenceId { get; set; }
        [Parameter]
        public short LastWarehouseId { get; set; }
        [Parameter]
        public int ProviderId { get; set; }
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrderDetail PurchaseOrderDetail;
        protected IEnumerable<ServiceModel.Warehouse> Warehouses;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected ServiceModel.ItemReference ItemReference;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                PurchaseOrderDetail = new ServiceModel.PurchaseOrderDetail();
                Warehouses = await WarehouseService.GetAsync();

                if (LastWarehouseId != 0)
                {
                    PurchaseOrderDetail.WarehouseId = LastWarehouseId;
                }
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
                if (PurchaseOrderDetails.Any(a => a.ReferenceId == PurchaseOrderDetail.ReferenceId && a.WarehouseId == PurchaseOrderDetail.WarehouseId))
                {
                    IsErrorVisible = true;
                    Error = "Ya existe una referencia para la misma bodega adicionada a esta orden de compra";
                    return;
                }
                if (!await PurchaseOrderDetailService.IsValidPurchaseOrderVariation(ProviderId,PurchaseOrderDetail.ReferenceId))
                    if (await DialogService.Confirm("Ha ingresado una cantidad fuera del rango promedio de ordenes de compra de la referencia " +
                            "con este proveedor. Desea continuar con el proceso?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar cantidad") == false) return; 
                PurchaseOrderDetail.Warehouse = Warehouses.Single(s => s.WarehouseId == PurchaseOrderDetail.WarehouseId);
                DialogService.Close(PurchaseOrderDetail);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task ItemReferenceHandler(ServiceModel.ItemReference reference)
        {
            PurchaseOrderDetail.ReferenceId = reference?.ReferenceId ?? 0;
            PurchaseOrderDetail.ItemReference = PurchaseOrderDetail.ReferenceId == 0 ? null : ProviderItemReferences.Single(s => s.ReferenceId == PurchaseOrderDetail.ReferenceId);
            ItemReference = reference;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}