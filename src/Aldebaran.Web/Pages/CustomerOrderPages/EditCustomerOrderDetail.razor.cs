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
        [Inject]
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }
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
        protected int originalRequestedQuantity; // Variable para almacenar la cantidad original

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                
                // Obtener la cantidad original real desde la base de datos
                await LoadOriginalRequestedQuantity();
                
                customerOrderDetail = new CustomerOrderDetail
                {
                    CustomerOrderId = CustomerOrderDetail.CustomerOrderId,
                    CustomerOrderDetailId = CustomerOrderDetail.CustomerOrderDetailId, // Corregir: debe ser CustomerOrderDetailId, no CustomerOrderId
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
            
            // Actualizar la cantidad original cada vez que se pasan nuevos parámetros
            if (CustomerOrderDetail?.CustomerOrderDetailId > 0)
            {
                await LoadOriginalRequestedQuantity();
            }
        }
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Obtiene la cantidad original real desde la base de datos
        /// </summary>
        private async Task LoadOriginalRequestedQuantity()
        {
            try
            {
                var customerOrderDetails = await CustomerOrderDetailService.GetByCustomerOrderIdAsync(CustomerOrderDetail.CustomerOrderId);
                var originalDetail = customerOrderDetails.FirstOrDefault(d => d.CustomerOrderDetailId == CustomerOrderDetail.CustomerOrderDetailId);
                
                originalRequestedQuantity = originalDetail?.RequestedQuantity ?? CustomerOrderDetail.RequestedQuantity;
            }
            catch
            {
                // En caso de error, usar el valor del parámetro como fallback
                originalRequestedQuantity = CustomerOrderDetail.RequestedQuantity;
            }
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

                // Calcular el stock disponible considerando que se "libera" la cantidad original del pedido
                // y luego se "reserva" la nueva cantidad
                var availableStock = localWarehouseStock.Quantity - customerOrderDetail.ItemReference.OrderedQuantity - customerOrderDetail.ItemReference.ReservedQuantity + originalRequestedQuantity;
                
                if (customerOrderDetail.RequestedQuantity > availableStock)
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