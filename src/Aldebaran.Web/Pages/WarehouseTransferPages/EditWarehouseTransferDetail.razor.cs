using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.WarehouseTransferPages
{
    public partial class EditWarehouseTransferDetail
    {
        #region Injections

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IWarehouseTransferDetailService WarehouseTransferDetailService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public WarehouseTransferDetail WarehouseTransferDetail { get; set; }

        [Parameter]
        public int WAREHOUSE_TRANSFER_ID { get; set; }

        [Parameter]
        public int REFERENCE_ID { get; set; }

        #endregion

        #region Global Variables

        bool hasREFERENCE_IDValue;

        public WarehouseTransferDetail warehouseTransferDetail { get; set; }

        protected bool IsErrorVisible;

        protected string Error;

        protected bool IsSubmitInProgress;

        bool hasWarehouseTransfer_IDValue;

        protected ItemReference ItemReference { get; set; }

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            warehouseTransferDetail = new WarehouseTransferDetail()
            {
                ItemReference = WarehouseTransferDetail.ItemReference,
                WarehouseTransfer = WarehouseTransferDetail.WarehouseTransfer,
                WarehouseTransferDetailId = WarehouseTransferDetail.WarehouseTransferDetailId,
                WarehouseTransferId = WarehouseTransferDetail.WarehouseTransferId,
                Quantity = WarehouseTransferDetail.Quantity,
                ReferenceId = WarehouseTransferDetail.ReferenceId             
            };

            ItemReference = await ItemReferenceService.FindAsync(WarehouseTransferDetail.ReferenceId);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            warehouseTransferDetail = new WarehouseTransferDetail() { };

            hasWarehouseTransfer_IDValue = parameters.TryGetValue<int>("WAREHOUSE_TRANSFER_ID", out var hasWarehouseTransfer_IDResult);

            if (hasWarehouseTransfer_IDValue)
            {
                warehouseTransferDetail.WarehouseTransferId = hasWarehouseTransfer_IDResult;
            }

            hasREFERENCE_IDValue = parameters.TryGetValue<int>("REFERENCE_ID", out var hasREFERENCE_IDResult);

            if (hasREFERENCE_IDValue)
            {
                warehouseTransferDetail.ReferenceId = hasREFERENCE_IDResult;
            }
                      
            await base.SetParametersAsync(parameters);
        }

        #endregion

        #region Events

        protected async Task FormSubmit()
        {
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;
                DialogService.Close(warehouseTransferDetail);
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        #endregion
    }
}