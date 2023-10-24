using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class ReferencePicker
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

        public short? LINE_ID { get; set; }
        public int? ITEM_ID { get; set; }

        [Parameter]
        public int? REFERENCE_ID { get; set; }
        [Parameter]
        public EventCallback<Models.AldebaranDb.ItemReference> OnChange { get; set; }

        protected IEnumerable<Models.AldebaranDb.Line> lines;
        protected Models.AldebaranDb.Line line;
        protected IEnumerable<Models.AldebaranDb.Item> items;
        protected Models.AldebaranDb.Item item;
        protected IEnumerable<Models.AldebaranDb.ItemReference> itemReferences;
        protected Models.AldebaranDb.ItemReference itemReference;
        protected override async Task OnInitializedAsync()
        {
            lines = await AldebaranDbService.GetLines();
        }
        protected bool CollapsedPanel { get; set; } = true;
        protected async Task OnLineChange(object lineId)
        {
            if (lineId == null)
            {
                line = null;
                CleanItems();
                await CleanReferences();
                return;
            }
            line = lines.Single(s => s.LINE_ID == (short)lineId);
            items = await AldebaranDbService.GetItems(new Query { Filter = $"i=>i.LINE_ID==@0", FilterParameters = new object[] { lineId } });
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
            itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = $"i=>i.ITEM_ID==@0", FilterParameters = new object[] { itemId } });
        }
        protected async Task OnReferenceChange(object referenceId)
        {
            if (referenceId == null)
            {
                itemReference = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            itemReference = itemReferences.Single(s => s.REFERENCE_ID == (int)referenceId);
            CollapsedPanel = true;
            await OnChange.InvokeAsync(itemReference);
        }
        protected async Task PanelCollapseToggle(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
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
        void CleanItems()
        {
            item = null;
            items = null;
        }
        async Task CleanReferences()
        {
            REFERENCE_ID = null;
            itemReference = null;
            itemReferences = null;
            await OnChange.InvokeAsync(null);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (REFERENCE_ID == null)
                return;
            var selectedReference = await AldebaranDbService.GetItemReferences(new Query { Filter = "i=>i.REFERENCE_ID == @0", FilterParameters = new object[] { REFERENCE_ID }, Expand = "Item.Line" });
            if (!selectedReference.Any())
                return;
            itemReference = selectedReference.First();
            LINE_ID = itemReference.Item.LINE_ID;
            await OnLineChange(LINE_ID);
            line = itemReference.Item.Line;
            ITEM_ID = itemReference.ITEM_ID;
            await OnItemChange(ITEM_ID);
            item = itemReference.Item;
        }
    }
}