using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class EditItemReference
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IItemService ItemService { get; set; }
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public int REFERENCE_ID { get; set; }

        [Parameter]
        public bool UPDATE_MINIMUM_QUANTITY { get; set; } = false;

        [Parameter]
        public bool PURCHASE_ORDER_VARIATION { get; set; } = false;

        [Parameter]
        public bool MINIMUM_QUANTITY_PERCENT { get; set; } = false;
        #endregion

        #region Variables
        protected ServiceModel.ItemReference ItemReference;
        protected ServiceModel.Item Item;

        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected bool IsReadOnlyFullEditing;
        protected List<string> ValidationErrors;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ItemReference = await ItemReferenceService.FindAsync(REFERENCE_ID);
                Item = ItemReference.Item;
                IsReadOnlyFullEditing = (UPDATE_MINIMUM_QUANTITY || PURCHASE_ORDER_VARIATION || MINIMUM_QUANTITY_PERCENT);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                ValidationErrors = new List<string>();

                var references = await ItemReferenceService.GetByItemIdAsync(ItemReference.ItemId);
                var nameAlreadyExists = references.Where(w => w.ReferenceId != REFERENCE_ID).Any(w => w.ReferenceName.Trim().ToLower() == ItemReference.ReferenceName.Trim().ToLower());

                if (!Item.IsDomesticProduct) 
                    if (!ItemReference.HavePurchaseOrderDetail)
                    {
                        if (ItemReference.AlarmMinimumQuantity <= 0 && ItemReference.MinimumQuantityPercent <= 0)
                            ValidationErrors.Add("Para los productos importados debe ingresar cantidad mínima o % cantidad mínima");
                    }
                    else
                        if (ItemReference.AlarmMinimumQuantity <= 0)
                            ValidationErrors.Add("Para los productos importados debe ingresar cantidad mínima");
                
                if (nameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe una referencia con el mismo nombre.");
                }
                var codeAlreadyExists = references.Where(w => w.ReferenceId != REFERENCE_ID).Any(w => w.ReferenceCode.Trim().ToLower() == ItemReference.ReferenceCode.Trim().ToLower());
                if (codeAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe una referencia con el mismo código.");
                }
                if (ValidationErrors.Any())
                {
                    IsErrorVisible = true;
                    return;
                }

                if (ItemReference.PurchaseOrderVariation == 0)
                    if (await DialogService.Confirm("No ha definido el porcentaje de variacion para las ordenes de compra. Desea continuar?", options: new ConfirmOptions { OkButtonText = "Si", CancelButtonText = "No" }, title: "Confirmar eliminación") == false)
                    {
                        return;
                    }

                await ItemReferenceService.UpdateAsync(REFERENCE_ID, ItemReference);                
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
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