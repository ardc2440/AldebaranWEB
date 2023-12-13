using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Shared
{
    public partial class ReferencePicker
    {

        #region Parameters
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public int? REFERENCE_ID { get; set; }
        [Parameter]
        public EventCallback<ItemReference> OnChange { get; set; }
        [Parameter]
        public IEnumerable<ItemReference> References { get; set; } = new List<ItemReference>();
        #endregion

        #region Variables
        protected IEnumerable<Line> lines;
        protected Line line;
        protected IEnumerable<Item> items;
        protected Item item;
        protected IEnumerable<ItemReference> itemReferences;
        protected ItemReference itemReference;
        public short? LINE_ID { get; set; }
        public int? ITEM_ID { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            lines = References.Select(s => s.Item.Line).Distinct();
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
            items = References.Where(w => w.Item.LINE_ID == (short)lineId).Select(s => s.Item).Distinct();
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
            itemReferences = References.Where(w => w.ITEM_ID == (int)itemId).Select(s => s);
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
            var selectedReference = References.Where(w => w.REFERENCE_ID == REFERENCE_ID).FirstOrDefault();
            if (selectedReference == null)
                return;
            itemReference = selectedReference;
            LINE_ID = itemReference.Item.LINE_ID;
            await OnLineChange(LINE_ID);
            line = itemReference.Item.Line;
            ITEM_ID = itemReference.ITEM_ID;
            await OnItemChange(ITEM_ID);
            item = itemReference.Item;
        }
    }
}