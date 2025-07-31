using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Web.Shared;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected IWarehouseStockValidationService WarehouseStockValidationService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }

        [Parameter]
        public int LastReferenceId { get; set; }

        #endregion

        #region Global Variables
        protected bool IsErrorVisible;
        protected string Error;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected CustomerOrderDetail customerOrderDetail;
        protected InventoryQuantities quantitiesPanel;
        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID { get; set; } = new List<ItemReference>();

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                itemReferencesForREFERENCEID = await ItemReferenceService.GetByStatusAsync(true);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerOrderDetail = new CustomerOrderDetail();

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
                
                if (CustomerOrderDetails.Any(ad => ad.ReferenceId == customerOrderDetail.ReferenceId))
                    throw new Exception("La referencia seleccionada ya existe dentro de esta reserva.");

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

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            customerOrderDetail.ReferenceId = reference?.ReferenceId ?? 0;
            customerOrderDetail.ItemReference = customerOrderDetail.ReferenceId == 0 ? null : itemReferencesForREFERENCEID.Single(s => s.ReferenceId == customerOrderDetail.ReferenceId);
        }

        protected async Task ValidateWarehouseStock()
        {
            if (customerOrderDetail?.ReferenceId > 0 && customerOrderDetail?.RequestedQuantity > 0)
            {
                try
                {
                    // Para un nuevo detalle de pedido, originalQuantity = 0
                    var validationResult = await WarehouseStockValidationService.ValidateLocalWarehouseStockAsync(
                        customerOrderDetail.ReferenceId, 
                        customerOrderDetail.RequestedQuantity, 
                        0);

                    if (!validationResult.IsValid)
                    {
                        // Guardar referencias importantes antes del alert para evitar que se pierdan
                        var tempReferenceId = customerOrderDetail.ReferenceId;
                        var tempRequestedQuantity = customerOrderDetail.RequestedQuantity;
                        var tempBrand = customerOrderDetail.Brand;
                        var tempItemReference = customerOrderDetail.ItemReference;
                        
                        // Validación informativa con estilo de warning - mostrar mensaje pero NO bloquear el proceso
                        await DialogService.Alert($"{validationResult.ErrorMessage}\n\nPuede continuar con el proceso normalmente.",
                            options: new AlertOptions() 
                            { 
                                OkButtonText = "Entendido"
                            }, 
                            title: "¡ADVERTENCIA!");

                        // Restaurar las referencias después del alert
                        customerOrderDetail.ReferenceId = tempReferenceId;
                        customerOrderDetail.RequestedQuantity = tempRequestedQuantity;
                        customerOrderDetail.Brand = tempBrand;
                        
                        // Solo restaurar ItemReference si se perdió
                        if (customerOrderDetail.ItemReference == null && tempItemReference != null)
                        {
                            customerOrderDetail.ItemReference = tempItemReference;
                        }
                        
                        // Si aún es null, buscarlo de nuevo
                        if (customerOrderDetail.ItemReference == null && tempReferenceId > 0)
                        {
                            customerOrderDetail.ItemReference = itemReferencesForREFERENCEID.FirstOrDefault(s => s.ReferenceId == tempReferenceId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // En caso de error en la validación, no bloquear el proceso
                }
            }
        }

        #endregion
    }
}