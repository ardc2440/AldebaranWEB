using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Application.Services.Services;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Linq.Dynamic.Core;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservationDetail
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
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        [Parameter]
        public int LastReferenceId { get; set; }

        #endregion

        #region Global Variables

        protected CustomerReservationDetail customerReservationDetail;
        protected InventoryQuantities QuantitiesPanel;
        protected bool IsErrorVisible;
        private bool Submitted;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected IEnumerable<ItemReference> ItemReferencesForREFERENCEID { get; set; } = new List<ItemReference>();
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ItemReferencesForREFERENCEID = await ItemReferenceService.GetByStatusAsync(true);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerReservationDetail = new CustomerReservationDetail();

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

                if (CustomerReservationDetails.Any(ad => ad.ReferenceId == customerReservationDetail.ReferenceId))
                    throw new Exception("La referencia seleccionada ya existe dentro de esta reserva.");

                // Validación informativa de stock - no bloquea el proceso
                await ValidateWarehouseStock();

                DialogService.Close(customerReservationDetail);
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
            customerReservationDetail.ReferenceId = reference?.ReferenceId ?? 0;
            customerReservationDetail.ItemReference = customerReservationDetail.ReferenceId == 0 ? null : ItemReferencesForREFERENCEID.Single(s => s.ReferenceId == customerReservationDetail.ReferenceId); ;
        }

        protected async Task ValidateWarehouseStock()
        {
            if (customerReservationDetail?.ReferenceId > 0 && customerReservationDetail?.ReservedQuantity > 0)
            {
                try
                {
                    // Para una nueva reserva, originalQuantity = 0
                    var validationResult = await WarehouseStockValidationService.ValidateLocalWarehouseStockAsync(
                        customerReservationDetail.ReferenceId, 
                        customerReservationDetail.ReservedQuantity, 
                        0);

                    if (!validationResult.IsValid)
                    {
                        // Guardar referencias importantes antes del alert
                        var tempReferenceId = customerReservationDetail.ReferenceId;
                        var tempReservedQuantity = customerReservationDetail.ReservedQuantity;
                        var tempBrand = customerReservationDetail.Brand;
                        var tempItemReference = customerReservationDetail.ItemReference;
                        
                        // Validación informativa con estilo de warning - mostrar mensaje pero NO bloquear el proceso
                        await DialogService.Alert($"{validationResult.ErrorMessage}\n\nPuede continuar con el proceso normalmente.",
                            options: new AlertOptions() 
                            { 
                                OkButtonText = "Entendido"
                            }, 
                            title: "¡ADVERTENCIA!");

                        // Restaurar las referencias después del alert
                        customerReservationDetail.ReferenceId = tempReferenceId;
                        customerReservationDetail.ReservedQuantity = tempReservedQuantity;
                        customerReservationDetail.Brand = tempBrand;
                        
                        // Restaurar ItemReference
                        if (customerReservationDetail.ItemReference == null && tempItemReference != null)
                        {
                            customerReservationDetail.ItemReference = tempItemReference;
                        }
                        
                        // Si aún es null, buscarlo de nuevo
                        if (customerReservationDetail.ItemReference == null && tempReferenceId > 0)
                        {
                            customerReservationDetail.ItemReference = ItemReferencesForREFERENCEID.FirstOrDefault(s => s.ReferenceId == tempReferenceId);
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