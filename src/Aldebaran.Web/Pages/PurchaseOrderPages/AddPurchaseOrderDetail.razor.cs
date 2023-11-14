using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class AddPurchaseOrderDetail
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public IEnumerable<ItemReference> ProviderItemReferences { get; set; } = new List<ItemReference>();

        protected bool errorVisible;
        protected PurchaseOrderDetail purchaseOrderDetail;
        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;
        protected bool isSubmitInProgress;
        protected InventoryQuantities QuantitiesPanel;

        protected override async Task OnInitializedAsync()
        {
            purchaseOrderDetail = new PurchaseOrderDetail();
            warehousesForWAREHOUSEID = await AldebaranDbService.GetWarehouses();
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                purchaseOrderDetail.Warehouse = await AldebaranDbService.GetWarehouseByWarehouseId(purchaseOrderDetail.WAREHOUSE_ID);
                var reference = await AldebaranDbService.GetItemReferences(new Query
                {
                    Filter = "i=> i.REFERENCE_ID==@0",
                    FilterParameters = new object[] { purchaseOrderDetail.REFERENCE_ID },
                    Expand = "Item.Line"
                });
                purchaseOrderDetail.ItemReference = reference.Single();
                DialogService.Close(purchaseOrderDetail);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }
        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            purchaseOrderDetail.REFERENCE_ID = reference?.REFERENCE_ID ?? 0;

            await QuantitiesPanel.Refresh(reference);
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}