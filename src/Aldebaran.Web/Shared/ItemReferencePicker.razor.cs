using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class ItemReferencePicker
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
        protected SecurityService Security { get; set; }

        [Inject]
        public AldebaranDbService AldebaranDbService { get; set; }

        [Parameter]
        public EventCallback<Models.AldebaranDb.ItemReference> OnChange { get; set; }

        protected bool CollapsedPanel { get; set; } = true;
        protected Models.AldebaranDb.Item item;
        protected Models.AldebaranDb.ItemReference reference;
        protected IEnumerable<Models.AldebaranDb.Item> items;
        protected IEnumerable<Models.AldebaranDb.ItemReference> references;
        public int? ITEM_ID { get; set; }
        public int? REFERENCE_ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            items = await AldebaranDbService.GetItems(new Query { Expand = "Line" });
        }
        protected async Task OnItemChange(object itemId)
        {
            if (itemId == null)
            {
                item = null;
                await CleanReferences();
                return;
            }
            item = items.Single(s => s.ITEM_ID == (int)itemId);
            references = await AldebaranDbService.GetItemReferences(new Query { Filter = $"i=>i.ITEM_ID==@0", FilterParameters = new object[] { itemId } });
        }
        protected async Task OnItemReferenceChange(object itemReferenceId)
        {
            if (itemReferenceId == null)
            {
                item = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            reference = references.Single(s => s.REFERENCE_ID == (int)itemReferenceId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(reference);
        }
        async Task CleanReferences()
        {
            reference = null;
            references = null;
            await OnChange.InvokeAsync(null);
        }

        protected async Task PanelCollapseToggle(MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void PanelCollapseChange(string Command)
        {
            if (Command == "Expand")
                CollapsedPanel = false;
            if (Command == "Collapse")
                CollapsedPanel = true;
        }
    }
}