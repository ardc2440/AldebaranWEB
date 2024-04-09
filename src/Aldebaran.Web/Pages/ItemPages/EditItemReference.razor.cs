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
        #endregion

        #region Variables
        protected ServiceModel.ItemReference ItemReference;
        protected ServiceModel.Item Item;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected bool IsErrorVisible;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                ItemReference = await ItemReferenceService.FindAsync(REFERENCE_ID);
                Item = ItemReference.Item;
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