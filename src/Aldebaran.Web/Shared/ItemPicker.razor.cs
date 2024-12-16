using Microsoft.AspNetCore.Components;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class ItemPicker
    {
        #region Parameters
        [Parameter]
        public int? ITEM_ID { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public EventCallback<ServiceModel.Item> OnChange { get; set; }
        [Parameter]
        public List<ServiceModel.Item> AvailableItemsForSelection { get; set; }
        #endregion

        #region Variables
        public short? LINE_ID { get; set; }
        protected ServiceModel.Line SelectedLine;
        protected IEnumerable<ServiceModel.Line> Lines = new List<ServiceModel.Line>();
        ItemData SelectedItem;
        IEnumerable<ItemData> Items = new List<ItemData>();
        protected bool CollapsedPanel { get; set; } = true;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Lines = AvailableItemsForSelection.Select(s => s.Line).DistinctBy(x => x.LineId);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            Lines = AvailableItemsForSelection.Select(s => s.Line).GroupBy(g => g.LineId).Select(s => s.First());
            if (ITEM_ID == null)
                return;
            var selectedItem = AvailableItemsForSelection.FirstOrDefault(w => w.ItemId == ITEM_ID.Value);
            if (selectedItem == null)
                return;

            await OnLineChange(selectedItem.LineId);
            await OnItemChange(selectedItem.ItemId);
        }
        #endregion

        #region Events
        protected async Task OnLineChange(object lineId)
        {
            if (lineId == null)
            {
                SelectedLine = null;
                CleanItems();
                await OnItemChange(null);
                return;
            }
            SelectedLine = Lines.Single(s => s.LineId == (short)lineId);
            Items = AvailableItemsForSelection
                        .Where(w => w.LineId == (short)lineId)
                        .Select(s => new ItemData
                        {
                            LineName = s.Line.LineName,
                            ItemId = s.ItemId,
                            ItemName = s.ItemName,
                            InternalReference = s.InternalReference,
                            FullName = $"{s.InternalReference} - {s.ItemName}"
                        });
        }
        protected async Task OnItemChange(object itemId)
        {
            if (itemId == null)
            {
                SelectedItem = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedItem = Items.Single(s => s.ItemId == (int)itemId);
            CollapsedPanel = true;
            IsSetParametersEnabled = false;
            await OnChange.InvokeAsync(AvailableItemsForSelection.FirstOrDefault(f => f.ItemId == SelectedItem.ItemId));
        }
        protected async Task PanelCollapseToggle(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void CleanItems()
        {
            Items = new List<ItemData>();
            ITEM_ID = null;
        }
        protected void PanelCollapseChange(string Command)
        {
            if (Command == "Expand")
                CollapsedPanel = false;
            if (Command == "Collapse")
                CollapsedPanel = true;
        }
        #endregion
    }

    class ItemData
    {
        public string LineName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string InternalReference { get; set; }
        public string FullName { get; set; }
    }
}
