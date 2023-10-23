using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class EditAdjustmentDetail
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

        [Parameter]
        public int ADJUSTMENT_DETAIL_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            adjustmentDetail = await AldebaranDbService.GetAdjustmentDetailByAdjustmentDetailId(ADJUSTMENT_DETAIL_ID);

            adjustmentsForADJUSTMENTID = await AldebaranDbService.GetAdjustments();

            itemReferencesForREFERENCEID = await AldebaranDbService.GetItemReferences();

            warehousesForWAREHOUSEID = await AldebaranDbService.GetWarehouses();
        }
        protected bool errorVisible;

        protected bool isSubmitInProgress;

        protected Models.AldebaranDb.AdjustmentDetail adjustmentDetail;

        protected IEnumerable<Models.AldebaranDb.Adjustment> adjustmentsForADJUSTMENTID;

        protected IEnumerable<Models.AldebaranDb.ItemReference> itemReferencesForREFERENCEID;

        protected IEnumerable<Models.AldebaranDb.Warehouse> warehousesForWAREHOUSEID;

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateAdjustmentDetail(ADJUSTMENT_DETAIL_ID, adjustmentDetail);
                DialogService.Close(adjustmentDetail);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
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
            adjustmentDetail = new Models.AldebaranDb.AdjustmentDetail();

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
    }
}