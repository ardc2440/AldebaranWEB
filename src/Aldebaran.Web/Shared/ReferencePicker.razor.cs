using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class ReferencePicker
    {
        #region Parameters
        [Parameter]
        public EventCallback<ServiceModel.ItemReference> OnChange { get; set; }
        [Parameter]
        public IEnumerable<ServiceModel.ItemReference> References { get; set; } = new List<ServiceModel.ItemReference>();
        [Parameter]
        public short? LINE_ID { get; set; }
        [Parameter]
        public int? ITEM_ID { get; set; }
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public int? REFERENCE_ID { get; set; }

        #endregion

        #region Variables
        protected IEnumerable<ServiceModel.Line> Lines = new List<ServiceModel.Line>();
        protected ServiceModel.Line SelectedLine;
        IEnumerable<ItemData> Items = new List<ItemData>();
        ItemData SelectedItem;
        protected IEnumerable<ServiceModel.ItemReference> ItemReferences = new List<ServiceModel.ItemReference>();
        protected ServiceModel.ItemReference SelectedItemReference;
        protected bool CollapsedPanel { get; set; } = true;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Lines = References.Select(s => s.Item.Line).GroupBy(g => g.LineId).Select(s => s.First()).OrderBy(o => o.LineName);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            Lines = References.Select(s => s.Item.Line).GroupBy(g => g.LineId).Select(s => s.First()).OrderBy(o => o.LineName);
            if (REFERENCE_ID == null)
                return;
            var selectedReference = References.Where(w => w.ReferenceId == REFERENCE_ID).FirstOrDefault();
            if (selectedReference == null)
                return;

            await OnLineChange(selectedReference.Item.LineId);
            await OnItemChange(selectedReference.Item.ItemId);
            await OnReferenceChange(selectedReference.ReferenceId);
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
            Items = References.Where(w => w.Item.LineId == (short)lineId).Select(s => s.Item)
                        .Select(s => new ItemData
                        {
                            LineName = s.Line.LineName,
                            ItemId = s.ItemId,
                            ItemName = s.ItemName,
                            InternalReference = s.InternalReference,
                            FullName = $"{s.InternalReference} - {s.ItemName}"
                        }).DistinctBy(w => w.ItemId).OrderBy(o => o.ItemName);
        }
        protected async Task OnItemChange(object itemId)
        {
            if (itemId == null)
            {
                SelectedItem = null;
                CleanReferences();
                await OnReferenceChange(null);
                return;
            }
            SelectedItem = Items.Single(s => s.ItemId == (int)itemId);
            ItemReferences = References.Where(w => w.ItemId == (int)itemId).Select(s => s).OrderBy(o => o.ReferenceName);
        }
        protected async Task OnReferenceChange(object referenceId)
        {
            if (referenceId == null)
            {
                SelectedItemReference = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedItemReference = ItemReferences.Single(s => s.ReferenceId == (int)referenceId);
            CollapsedPanel = true;
            IsSetParametersEnabled = false;
            await OnChange.InvokeAsync(SelectedItemReference);
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
        void CleanReferences()
        {
            ItemReferences = new List<ServiceModel.ItemReference>();
            REFERENCE_ID = null;
        }
        void PanelCollapseChange(string Command)
        {
            if (Command == "Expand")
                CollapsedPanel = false;
            if (Command == "Collapse")
                CollapsedPanel = true;
        }
        #endregion
    }
}