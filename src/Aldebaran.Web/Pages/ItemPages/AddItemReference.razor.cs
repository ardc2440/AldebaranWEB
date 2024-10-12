using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class AddItemReference
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
        public int ITEM_ID { get; set; }
        #endregion

        #region Variables
        protected ServiceModel.ItemReference ItemReference;
        protected ServiceModel.Item Item;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        protected List<string> ValidationErrors;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                Item = await ItemService.FindAsync(ITEM_ID);
                ItemReference = new ServiceModel.ItemReference
                {
                    ItemId = ITEM_ID,
                    InventoryQuantity = 0,
                    OrderedQuantity = 0,
                    ReservedQuantity = 0,
                    WorkInProcessQuantity = 0
                };
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
                var referenceNameAlreadyExists = await ItemReferenceService.ExistsByReferenceName(ItemReference.ReferenceName, ItemReference.ItemId);
                if (!Item.IsDomesticProduct)
                    if (ItemReference.AlarmMinimumQuantity <= 0 && ItemReference.MinimumQuantityPercent <= 0)
                        ValidationErrors.Add("Para los productos importados debe ingresar cantidad mínima o % cantidad mínima");
                
                if (referenceNameAlreadyExists)
                {
                    ValidationErrors.Add("Ya existe una referencia con el mismo nombre.");
                }
                var referenceCodeAlreadyExists = await ItemReferenceService.ExistsByReferenceCode(ItemReference.ReferenceCode, ItemReference.ItemId);
                if (referenceCodeAlreadyExists)
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

                await ItemReferenceService.AddAsync(ItemReference);
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