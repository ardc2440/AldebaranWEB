using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class EditCustomerOrderDetail
    {
        #region Injection
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected IWarehouseService WarehouseService { get; set; }
        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }
        #endregion

        #region Parameters

        [Parameter]
        public CustomerOrderDetail CustomerOrderDetail { get; set; }

        #endregion

        #region Global Variables
        protected bool IsErrorVisible;
        protected string Error;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected ItemReference ItemReference { get; set; }
        protected CustomerOrderDetail customerOrderDetail { get; set; }

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                customerOrderDetail = new CustomerOrderDetail
                {
                    CustomerOrderId = CustomerOrderDetail.CustomerOrderId,
                    CustomerOrderDetailId = CustomerOrderDetail.CustomerOrderId,
                    CustomerOrder = CustomerOrderDetail.CustomerOrder,
                    Brand = CustomerOrderDetail.Brand,
                    ItemReference = CustomerOrderDetail.ItemReference,
                    ReferenceId = CustomerOrderDetail.ReferenceId,
                    RequestedQuantity = CustomerOrderDetail.RequestedQuantity,
                    ProcessedQuantity = CustomerOrderDetail.ProcessedQuantity,
                    DeliveredQuantity = CustomerOrderDetail.DeliveredQuantity
                };

                ItemReference = await ItemReferenceService.FindAsync(CustomerOrderDetail.ReferenceId);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            CustomerOrderDetail = new CustomerOrderDetail();

            await base.SetParametersAsync(parameters);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            await ValidateWarehouseStock();

            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                DialogService.Close(customerOrderDetail);
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

        protected async Task ValidateWarehouseStock()
        {
            if (!(customerOrderDetail.ItemReference.Item.IsSpecialImport || customerOrderDetail.ItemReference.Item.IsDomesticProduct))
            {
                var warehouse = await WarehouseService.FindByCodeAsync(1);
                var localWarehouseStock = await ReferencesWarehouseService.GetByReferenceAndWarehouseIdAsync(customerOrderDetail.ReferenceId, warehouse.WarehouseId);

                if (customerOrderDetail.RequestedQuantity > localWarehouseStock.Quantity - customerOrderDetail.ItemReference.OrderedQuantity - customerOrderDetail.ItemReference.ReservedQuantity)
                {
                    var temp = customerOrderDetail;
                    await DialogService.Alert($"La cantidad ingresada supera la existencia en bodega local. Verifique disponibilidad de la referencia.",
                        options: new AlertOptions() { OkButtonText = "Cerrar" }, title: "Stock en bodega local");

                    customerOrderDetail = temp;
                    StateHasChanged();
                }
            }
        }
        #endregion

    }
}