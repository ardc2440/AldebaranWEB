using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AddAdjustmentDetail
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

        protected bool errorVisible;

        protected string alertMessage = "No se ha podido Agregar la Referencia";

        protected AdjustmentDetail adjustmentDetail;

        protected IEnumerable<Adjustment> adjustmentsForADJUSTMENTID;

        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID;

        protected IEnumerable<Warehouse> warehousesForWAREHOUSEID;

        [Parameter]
        public ICollection<AdjustmentDetail> adjustmentDetails { get; set; }

        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            adjustmentsForADJUSTMENTID = await AldebaranDbService.GetAdjustments();

            itemReferencesForREFERENCEID = await AldebaranDbService.GetItemReferences();

            warehousesForWAREHOUSEID = await AldebaranDbService.GetWarehouses();

            adjustmentDetail.QUANTITY = 1;
        }

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (adjustmentDetails.Any(ad => ad.REFERENCE_ID.Equals(adjustmentDetail.REFERENCE_ID) && ad.WAREHOUSE_ID.Equals(adjustmentDetail.WAREHOUSE_ID)))
                    throw new Exception("La Referencia y Bodega seleccionadas, ya existen dentro de este ajuste.");

                adjustmentDetail.Warehouse = await AldebaranDbService.GetWarehouseByWarehouseId(adjustmentDetail.WAREHOUSE_ID);
                var reference = await AldebaranDbService.GetItemReferences(new Query { Filter = "i=> i.REFERENCE_ID==@0", FilterParameters = new object[] { adjustmentDetail.REFERENCE_ID }, Expand = "Item" });
                adjustmentDetail.ItemReference = reference.Single();
                DialogService.Close(adjustmentDetail);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        bool hasADJUSTMENT_IDValue;

        [Parameter]
        public int ADJUSTMENT_ID { get; set; }

        bool hasREFERENCE_IDValue;

        [Parameter]
        public int REFERENCE_ID { get; set; }

        bool hasWAREHOUSE_IDValue;

        [Parameter]
        public short WAREHOUSE_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            adjustmentDetail = new AdjustmentDetail();

            hasADJUSTMENT_IDValue = parameters.TryGetValue<int>("ADJUSTMENT_ID", out var hasADJUSTMENT_IDResult);

            if (hasADJUSTMENT_IDValue)
            {
                adjustmentDetail.ADJUSTMENT_ID = hasADJUSTMENT_IDResult;
            }

            hasREFERENCE_IDValue = parameters.TryGetValue<int>("REFERENCE_ID", out var hasREFERENCE_IDResult);

            if (hasREFERENCE_IDValue)
            {
                adjustmentDetail.REFERENCE_ID = hasREFERENCE_IDResult;
            }

            hasWAREHOUSE_IDValue = parameters.TryGetValue<short>("WAREHOUSE_ID", out var hasWAREHOUSE_IDResult);

            if (hasWAREHOUSE_IDValue)
            {
                adjustmentDetail.WAREHOUSE_ID = hasWAREHOUSE_IDResult;
            }
            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            adjustmentDetail.REFERENCE_ID = reference?.REFERENCE_ID ?? 0;
        }
    }
}