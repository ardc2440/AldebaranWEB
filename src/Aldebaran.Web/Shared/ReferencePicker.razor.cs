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
        protected IEnumerable<ServiceModel.Line> Lines;
        protected ServiceModel.Line SelectedLine;
        protected IEnumerable<ServiceModel.Item> Items;
        protected ServiceModel.Item SelectedItem;
        protected IEnumerable<ServiceModel.ItemReference> ItemReferences;
        protected ServiceModel.ItemReference SelectedItemReference;
        protected bool CollapsedPanel { get; set; } = true;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            Lines = References.Select(s => s.Item.Line).GroupBy(g => g.LineId).Select(s => s.First());
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            Lines = References.Select(s => s.Item.Line).GroupBy(g => g.LineId).Select(s => s.First());
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
                Items = null;
                SelectedItem = null;
                ItemReferences = null;
                SelectedItemReference = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedLine = Lines.Single(s => s.LineId == (short)lineId);
            Items = References.Where(w => w.Item.LineId == (short)lineId).Select(s => s.Item).DistinctBy(w => w.ItemId);
        }
        protected async Task OnItemChange(object itemId)
        {
            if (itemId == null)
            {
                SelectedItem = null;
                ItemReferences = null;
                SelectedItemReference = null;
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedItem = Items.Single(s => s.ItemId == (int)itemId);
            ItemReferences = References.Where(w => w.ItemId == (int)itemId).Select(s => s);
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