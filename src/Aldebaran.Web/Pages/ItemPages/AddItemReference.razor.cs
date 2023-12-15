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
        protected bool ErrorVisible;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Item = await ItemService.FindAsync(ITEM_ID);
            ItemReference = new ServiceModel.ItemReference
            {
                ItemId = ITEM_ID
            };
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                await ItemReferenceService.AddAsync(ItemReference);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                ErrorVisible = true;
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