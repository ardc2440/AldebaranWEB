using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
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
        protected IWarehouseService WarehouseService { get; set; }
        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }
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
            await ValidateWarehouseStock();

            try
            {
                IsErrorVisible = false;
                IsSubmitInProgress = true;
                
                if (CustomerOrderDetails.Any(ad => ad.ReferenceId == customerOrderDetail.ReferenceId))
                    throw new Exception("La referencia seleccionada ya existe dentro de esta reserva.");

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