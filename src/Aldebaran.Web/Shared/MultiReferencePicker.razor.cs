using Microsoft.AspNetCore.Components;
using Radzen;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Shared
{
    public partial class MultiReferencePicker
    {
        #region Parameters
        [Parameter]
        public bool ReadOnly { get; set; } = false;
        [Parameter]
        public EventCallback<List<ServiceModel.ItemReference>> OnChange { get; set; }
        #endregion

        #region Variables
        public List<ServiceModel.ItemReference> AvailableItemReferencesForSelection { get; set; }
        List<GroupReferenceData> References = new List<GroupReferenceData>();
        List<GroupItemData> Items = new List<GroupItemData>();

        protected List<short> SelectedLineIds = new List<short>();
        protected List<int> SelectedItemIds = new List<int>();
        protected List<int> SelectedReferenceIds = new List<int>();
        protected List<ServiceModel.ItemReference> SelectedReferences = new List<ServiceModel.ItemReference>();
        protected bool CollapsedPanel { get; set; } = true;
        #endregion

        #region Overrides
        public void SetAvailableItemReferencesForSelection(List<ServiceModel.ItemReference> references)
        {
            CleanLines();
            CleanItems();
            CleanReferences();
            AvailableItemReferencesForSelection = references;
            CollapsedPanel = (references?.Any()) != true;
        }
        public void SetSelectedItemReferences(List<int> referenceIds)
        {
            if (AvailableItemReferencesForSelection?.Any() == true && referenceIds?.Any() == true)
            {
                var selectedLines = AvailableItemReferencesForSelection.Where(w => referenceIds.Contains(w.ReferenceId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
                var selectedItems = AvailableItemReferencesForSelection.Where(w => referenceIds.Contains(w.ReferenceId)).Select(s => s.Item).DistinctBy(d => d.ItemId).ToList();
                var selectedReferences = AvailableItemReferencesForSelection.Where(w => referenceIds.Contains(w.ReferenceId)).ToList();

                SelectedLineIds = selectedLines.Select(s => s.LineId).Distinct().ToList();
                OnLineChange();
                SelectedItemIds = selectedItems.Select(s => s.ItemId).Distinct().ToList();
                OnItemChange();
                SelectedReferenceIds = selectedReferences.Select(s => s.ReferenceId).Distinct().ToList();
                OnReferenceChange();
                CollapsedPanel = false;
            }
            else
            {
                CleanLines();
                CleanItems();
                CleanReferences();
            }
        }
        #endregion

        #region Events
        protected void OnLineChange()
        {
            if (SelectedLineIds == null || !SelectedLineIds.Any())
            {
                CleanItems();
                OnItemChange();
                return;
            }

            var selectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var itemBySelectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item).DistinctBy(w => w.ItemId).ToList();

            Items = itemBySelectedLines.OrderBy(o => o.Line.LineName).ThenBy(o => o.ItemName)
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
        protected void OnItemChange()
        {
            if (SelectedItemIds == null || !SelectedItemIds.Any())
            {
                CleanReferences();
                OnReferenceChange();
                return;
            }

            var selectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var selectedItems = AvailableItemReferencesForSelection.Where(w => SelectedItemIds.Contains(w.ItemId)).Select(s => s.Item).DistinctBy(d => d.ItemId).ToList();
            var referencesBySelectedItems = AvailableItemReferencesForSelection.Where(w => SelectedItemIds.Contains(w.Item.ItemId)).DistinctBy(d => d.ReferenceId).ToList();

            References = referencesBySelectedItems.OrderBy(o => o.Item.ItemName).ThenBy(o => o.ReferenceName)
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
        protected void OnReferenceChange()
        {
            if (SelectedReferenceIds == null || !SelectedReferenceIds.Any())
            {
                OnChange.InvokeAsync(null);
                return;
            }
            SelectedReferences = AvailableItemReferencesForSelection.Where(w => SelectedReferenceIds.Contains(w.ReferenceId)).ToList();
            OnChange.InvokeAsync(SelectedReferences);
        }
        protected async Task PanelCollapseToggle(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            CollapsedPanel = !CollapsedPanel;
        }
        void CleanLines()
        {
            SelectedLineIds = new List<short>();
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