using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
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
        protected ICustomerOrderDetailService CustomerOrderDetailService { get; set; }
        [Inject]
        protected IWarehouseStockValidationService WarehouseStockValidationService { get; set; }
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
                    CustomerOrderDetailId = CustomerOrderDetail.CustomerOrderDetailId,
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
            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;

                // Validación informativa de stock - no bloquea el proceso
                await ValidateWarehouseStock();

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
            if (customerOrderDetail?.ReferenceId > 0 && customerOrderDetail?.RequestedQuantity > 0)
            {
                try
                {
                    // Determinar si estamos en modo "crear pedido" o "editar pedido existente"
                    bool isNewOrder = customerOrderDetail.CustomerOrderDetailId == 0;
                    
                    if (isNewOrder)
                    {
                        // CASO 1: Estamos creando un pedido nuevo (editando detalles antes de guardar)
                        // La cantidad original es siempre 0 porque aún no está en la BD
                        var validationResult = await WarehouseStockValidationService.ValidateLocalWarehouseStockAsync(
                            customerOrderDetail.ReferenceId, 
                            customerOrderDetail.RequestedQuantity, // Validamos toda la cantidad solicitada
                            0); // No hay cantidad original porque es nuevo

                        if (!validationResult.IsValid)
                        {
                            await ShowWarningMessage(validationResult.ErrorMessage);
                        }
                    }
                    else
                    {
                        // CASO 2: Estamos editando un pedido existente guardado en BD
                        // Para stock negativo o insuficiente, validar siempre la cantidad total
                        
                        // Primero verificar si hay suficiente stock para la cantidad total solicitada
                        var totalValidationResult = await WarehouseStockValidationService.ValidateLocalWarehouseStockAsync(
                            customerOrderDetail.ReferenceId, 
                            customerOrderDetail.RequestedQuantity, 
                            originalRequestedQuantity); // Liberar la cantidad original

                        if (!totalValidationResult.IsValid)
                        {
                            await ShowWarningMessage(totalValidationResult.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error en la validación, no bloquear el proceso
                }
            }
        }

        /// <summary>
        /// Muestra el mensaje de warning y restaura las propiedades después del alert
        /// </summary>
        private async Task ShowWarningMessage(string errorMessage)
        {
            // Guardar referencias importantes antes del alert
            var tempReferenceId = customerOrderDetail.ReferenceId;
            var tempRequestedQuantity = customerOrderDetail.RequestedQuantity;
            var tempBrand = customerOrderDetail.Brand;
            var tempItemReference = customerOrderDetail.ItemReference;
            var tempCustomerOrderDetailId = customerOrderDetail.CustomerOrderDetailId;
            var tempCustomerOrderId = customerOrderDetail.CustomerOrderId;
            var tempProcessedQuantity = customerOrderDetail.ProcessedQuantity;
            var tempDeliveredQuantity = customerOrderDetail.DeliveredQuantity;
            
            // Validación informativa con estilo de warning - mostrar mensaje pero NO bloquear el proceso
            await DialogService.Alert($"{errorMessage}\n\nPuede continuar con el proceso normalmente.",
                options: new AlertOptions() 
                { 
                    OkButtonText = "Entendido"
                }, 
                title: "¡ADVERTENCIA!");

            // Restaurar todas las propiedades después del alert
            customerOrderDetail.ReferenceId = tempReferenceId;
            customerOrderDetail.RequestedQuantity = tempRequestedQuantity;
            customerOrderDetail.Brand = tempBrand;
            customerOrderDetail.CustomerOrderDetailId = tempCustomerOrderDetailId;
            customerOrderDetail.CustomerOrderId = tempCustomerOrderId;
            customerOrderDetail.ProcessedQuantity = tempProcessedQuantity;
            customerOrderDetail.DeliveredQuantity = tempDeliveredQuantity;
            
            // Restaurar ItemReference
            if (customerOrderDetail.ItemReference == null && tempItemReference != null)
            {
                customerOrderDetail.ItemReference = tempItemReference;
            }
        }
        #endregion

    }
}