using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ItemPages
{
    public partial class AddItemReference
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Parameter]
        public int ITEM_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            item = await AldebaranDbService.GetItemByItemId(ITEM_ID);
            itemReference = new Models.AldebaranDb.ItemReference();
            itemReference.ITEM_ID = ITEM_ID;
        }
        protected bool errorVisible;
        protected Models.AldebaranDb.ItemReference itemReference;

        protected Models.AldebaranDb.Item item;
        protected bool isSubmitInProgress;

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateItemReference(itemReference);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}