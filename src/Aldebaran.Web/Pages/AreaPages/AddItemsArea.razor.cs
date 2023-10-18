using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.AreaPages
{
    public partial class AddItemsArea
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

        [Parameter]
        public short AREA_ID { get; set; }

        protected bool errorVisible;
        protected Models.AldebaranDb.ItemsArea itemsArea;
        protected Models.AldebaranDb.Area area;
        protected IEnumerable<Models.AldebaranDb.Item> itemsForITEMID;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            area = await AldebaranDbService.GetAreaByAreaId(AREA_ID);
            var currentItemsInArea = await AldebaranDbService.GetItemsAreas(new Query { Filter = $"@i => i.AREA_ID == @0", FilterParameters = new object[] { AREA_ID } });
            itemsForITEMID = await AldebaranDbService.GetItems(new Query { Filter = $"@i => !@0.Contains(i.ITEM_ID)", FilterParameters = new object[] { currentItemsInArea.Select(s => s.ITEM_ID) } });
            itemsArea = new Models.AldebaranDb.ItemsArea();
            itemsArea.AREA_ID = AREA_ID;
        }
        
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateItemsArea(itemsArea);
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