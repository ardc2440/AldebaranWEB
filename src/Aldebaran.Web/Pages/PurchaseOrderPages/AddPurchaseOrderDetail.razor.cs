using Aldebaran.Application.Services;
using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Shared;
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
        #endregion

        #region Parameters
        [Parameter]
        public IEnumerable<ServiceModel.ItemReference> ProviderItemReferences { get; set; } = new List<ServiceModel.ItemReference>();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected ServiceModel.PurchaseOrderDetail PurchaseOrderDetail;
        protected IEnumerable<ServiceModel.Warehouse> Warehouses;
        protected bool IsSubmitInProgress;
        protected InventoryQuantities InventoryQuantitiesPanel;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            PurchaseOrderDetail = new ServiceModel.PurchaseOrderDetail();
            Warehouses = await WarehouseService.GetAsync();
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
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
            await InventoryQuantitiesPanel.Refresh(reference);
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        #endregion
    }
}