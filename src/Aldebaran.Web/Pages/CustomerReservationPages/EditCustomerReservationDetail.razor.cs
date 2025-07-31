using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class EditCustomerReservationDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        [Inject]
        protected IWarehouseStockValidationService WarehouseStockValidationService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public CustomerReservationDetail CustomerReservationDetail { get; set; }

        #endregion

        #region Global Variables

        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;                       
        protected string Error;
        protected CustomerReservationDetail CustomerReservationDetailData { get; set; }
        protected ItemReference ItemReference { get; set; }
        protected int originalReservedQuantity; // Variable para almacenar la cantidad original

        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                // Obtener la cantidad original real desde la base de datos
                await LoadOriginalReservedQuantity();

                CustomerReservationDetailData = new CustomerReservationDetail
                {
                    Brand = CustomerReservationDetail.Brand,
                    CustomerReservationDetailId = CustomerReservationDetail.CustomerReservationDetailId,
                    ReferenceId = CustomerReservationDetail.ReferenceId,
                    ItemReference = CustomerReservationDetail.ItemReference,
                    ReservedQuantity = CustomerReservationDetail.ReservedQuantity,
                    CustomerReservation = CustomerReservationDetail.CustomerReservation,
                    SendToCustomerOrder = CustomerReservationDetail.SendToCustomerOrder,
                    CustomerReservationId = CustomerReservationDetail.CustomerReservationId
                };

                ItemReference = await ItemReferenceService.FindAsync(CustomerReservationDetail.ReferenceId);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            CustomerReservationDetail = new CustomerReservationDetail();

            await base.SetParametersAsync(parameters);

            // Actualizar la cantidad original cada vez que se pasan nuevos parámetros
            if (CustomerReservationDetail?.CustomerReservationDetailId > 0)
            {
                await LoadOriginalReservedQuantity();
            }
        }
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Obtiene la cantidad reservada original real desde la base de datos
        /// </summary>
        private async Task LoadOriginalReservedQuantity()
        {
            try
            {
                var originalDetail = await CustomerReservationDetailService.FindAsync(CustomerReservationDetail.CustomerReservationDetailId);
                originalReservedQuantity = originalDetail?.ReservedQuantity ?? CustomerReservationDetail.ReservedQuantity;
            }
            catch
            {
                // En caso de error, usar el valor del parámetro como fallback
                originalReservedQuantity = CustomerReservationDetail.ReservedQuantity;
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

                DialogService.Close(CustomerReservationDetailData);
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
            if (CustomerReservationDetailData?.ReferenceId > 0 && CustomerReservationDetailData?.ReservedQuantity > 0)
            {
                try
                {
                    // Para reservas, siempre validar la cantidad total cuando el stock puede ser insuficiente
                    // incluso si es una reducción, porque el stock puede estar negativo
                    
                    var validationResult = await WarehouseStockValidationService.ValidateLocalWarehouseStockAsync(
                        CustomerReservationDetailData.ReferenceId, 
                        CustomerReservationDetailData.ReservedQuantity, 
                        originalReservedQuantity); // Liberar la cantidad original

                    if (!validationResult.IsValid)
                    {
                        // Guardar referencias importantes antes del alert
                        var tempReferenceId = CustomerReservationDetailData.ReferenceId;
                        var tempReservedQuantity = CustomerReservationDetailData.ReservedQuantity;
                        var tempBrand = CustomerReservationDetailData.Brand;
                        var tempItemReference = CustomerReservationDetailData.ItemReference;
                        var tempDetailId = CustomerReservationDetailData.CustomerReservationDetailId;
                        var tempReservationId = CustomerReservationDetailData.CustomerReservationId;
                        var tempSendToCustomerOrder = CustomerReservationDetailData.SendToCustomerOrder;
                        
                        // Validación informativa con estilo de warning - mostrar mensaje pero NO bloquear el proceso
                        await DialogService.Alert($"{validationResult.ErrorMessage}\n\nPuede continuar con el proceso normalmente.",
                            options: new AlertOptions() 
                            { 
                                OkButtonText = "Entendido"
                            }, 
                            title: "¡ADVERTENCIA!");

                        // Restaurar todas las propiedades después del alert
                        CustomerReservationDetailData.ReferenceId = tempReferenceId;
                        CustomerReservationDetailData.ReservedQuantity = tempReservedQuantity;
                        CustomerReservationDetailData.Brand = tempBrand;
                        CustomerReservationDetailData.CustomerReservationDetailId = tempDetailId;
                        CustomerReservationDetailData.CustomerReservationId = tempReservationId;
                        CustomerReservationDetailData.SendToCustomerOrder = tempSendToCustomerOrder;
                        
                        // Restaurar ItemReference
                        if (CustomerReservationDetailData.ItemReference == null && tempItemReference != null)
                        {
                            CustomerReservationDetailData.ItemReference = tempItemReference;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error en la validación, no bloquear el proceso
                    Console.WriteLine($"Error en validación de stock: {ex.Message}");
                }
            }
        }

        #endregion
    }
}