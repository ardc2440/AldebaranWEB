using Aldebaran.Application.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class MultiReferencePicker
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public EventCallback<List<ServiceModel.ItemReference>> OnChange { get; set; }
        [Parameter]
        public List<int> SELECTED_REFERENCES { get; set; } = new List<int>();
        #endregion

        #region Variables
        public List<ServiceModel.ItemReference> AvailableItemReferencesForSelection { get; set; }

        protected List<ServiceModel.Line> Lines = new List<ServiceModel.Line>();
        protected List<short> SelectedLineIds = new List<short>();

        List<GroupItemData> Items = new List<GroupItemData>();
        protected List<int> SelectedItemIds = new List<int>();

        List<GroupReferenceData> References = new List<GroupReferenceData>();
        protected List<int> SelectedReferenceIds = new List<int>();
        protected List<ServiceModel.ItemReference> SelectedReferences = new List<ServiceModel.ItemReference>();

        protected bool CollapsedPanel { get; set; } = true;
        bool IsSetParametersEnabled = true;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            AvailableItemReferencesForSelection = (await ItemReferenceService.GetAsync()).ToList();
            Lines = AvailableItemReferencesForSelection.Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!IsSetParametersEnabled) return;
            if (SELECTED_REFERENCES == null || !SELECTED_REFERENCES.Any())
                return;

            var selectedLines = AvailableItemReferencesForSelection.Where(w => SELECTED_REFERENCES.Contains(w.ReferenceId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var selectedItems = AvailableItemReferencesForSelection.Where(w => SELECTED_REFERENCES.Contains(w.ReferenceId)).Select(s => s.Item).DistinctBy(d => d.ItemId).ToList();
            var selectedReferences = AvailableItemReferencesForSelection.Where(w => SELECTED_REFERENCES.Contains(w.ReferenceId)).ToList();

            SelectedLineIds = selectedLines.Select(s => s.LineId).Distinct().ToList();
            await OnLineChange();
            SelectedItemIds = selectedItems.Select(s => s.ItemId).Distinct().ToList();
            await OnItemChange();
            SelectedReferenceIds = selectedReferences.Select(s => s.ReferenceId).Distinct().ToList();
            await OnReferenceChange();
            CollapsedPanel = true;
        }
        #endregion

        #region Events
        protected async Task OnLineChange()
        {
            if (SelectedLineIds == null || !SelectedLineIds.Any())
            {
                CleanItems();
                await OnItemChange();
                return;
            }

            var selectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var itemBySelectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item).DistinctBy(w => w.ItemId).ToList();

            Items = itemBySelectedLines
                .GroupBy(g => g.LineId)
                .SelectMany(s => new GroupItemData[] {
                    new() {
                        LineName = selectedLines.First(f=>f.LineId == s.Key).LineName
                    }
                }.Concat(s.Select(x => new GroupItemData
                {
                    ItemId = x.ItemId,
                    ItemName = x.ItemName
                }))).ToList();
        }
        protected async Task OnItemChange()
        {
            if (SelectedItemIds == null || !SelectedItemIds.Any())
            {
                CleanReferences();
                await OnReferenceChange();
                return;
            }

            var selectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var selectedItems = AvailableItemReferencesForSelection.Where(w => SelectedItemIds.Contains(w.ItemId)).Select(s => s.Item).DistinctBy(d => d.ItemId).ToList();
            var referencesBySelectedItems = AvailableItemReferencesForSelection.Where(w => SelectedItemIds.Contains(w.Item.ItemId)).DistinctBy(d => d.ReferenceId).ToList();

            References = referencesBySelectedItems
                .GroupBy(g => new { g.Item.LineId, g.Item.ItemId })
                .SelectMany(s => new GroupReferenceData[] {
                     new () {
                        LineName = selectedLines.First(f=>f.LineId == s.Key.LineId).LineName,
                        ItemName = selectedItems.First(f=>f.ItemId == s.Key.ItemId).ItemName,
                     }
                }.Concat(s.Select(x => new GroupReferenceData
                {
                    ReferenceId = x.ReferenceId,
                    ReferenceName = x.ReferenceName,
                    FullReferenceName = $"{x.Item.ItemName} - {x.ReferenceName}"
                }))).ToList();
        }
        protected async Task OnReferenceChange()
        {
            if (SelectedReferenceIds == null || !SelectedReferenceIds.Any())
            {
                await OnChange.InvokeAsync(null);
                return;
            }
            SelectedReferences = AvailableItemReferencesForSelection.Where(w => SelectedReferenceIds.Contains(w.ReferenceId)).ToList();
            IsSetParametersEnabled = false;
            await OnChange.InvokeAsync(SelectedReferences);
        }
        protected async Task PanelCollapseToggle(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void CleanItems()
        {
            Items = new List<GroupItemData>();
            SelectedItemIds = new List<int>();
        }
        void CleanReferences()
        {
            References = new List<GroupReferenceData>();
            SelectedReferenceIds = new List<int>();
            SelectedReferences = new List<ServiceModel.ItemReference>();
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
    class GroupItemData
    {
        public string LineName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public bool IsGroup { get { return LineName != null; } }
    }
    class GroupReferenceData
    {
        public string LineName { get; set; }
        public string ItemName { get; set; }
        public int ReferenceId { get; set; }
        public string ReferenceName { get; set; }
        public bool IsGroup { get { return ItemName != null; } }
        public string FullReferenceName { get; set; }
    }
}