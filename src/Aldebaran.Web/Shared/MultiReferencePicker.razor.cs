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
        protected IEnumerable<ServiceModel.Line> Lines = new List<ServiceModel.Line>();
        public List<ServiceModel.ItemReference> AvailableItemReferencesForSelection { get; set; }
        List<GroupReferenceData> References = new();
        List<GroupItemData> Items = new();

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
            Lines = AvailableItemReferencesForSelection?.Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList().OrderBy(o => o.LineName);
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
            CleanItems();            
            OnItemChange();
            if (SelectedLineIds == null || !SelectedLineIds.Any())
            {
                OnChange.InvokeAsync(null);
                return;
            }
            var selectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item.Line).DistinctBy(d => d.LineId).ToList();
            var itemBySelectedLines = AvailableItemReferencesForSelection.Where(w => SelectedLineIds.Contains(w.Item.LineId)).Select(s => s.Item).DistinctBy(w => w.ItemId).ToList();

            Items = itemBySelectedLines.OrderBy(o => o.Line.LineName).ThenBy(o => o.ItemName)
                .GroupBy(g => g.LineId)
                .SelectMany(s => new GroupItemData[] {
                    new() {
                        LineName = selectedLines.First(f=>f.LineId == s.Key).LineName,
                        ItemName = string.Empty
                    }
                }.Concat(s.Select(x => new GroupItemData
                {
                    ItemId = x.ItemId,
                    ItemName = x.ItemName
                }))).ToList();
        }
        protected void OnItemChange()
        {
            CleanReferences();            
            OnReferenceChange();
            if (SelectedItemIds == null || !SelectedItemIds.Any())
            {
                OnChange.InvokeAsync(null);
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
                        ReferenceName = selectedItems.First(f=>f.ItemId == s.Key.ItemId).ItemName,
                     }
                }.Concat(s.Select(x => new GroupReferenceData
                {
                    ReferenceId = x.ReferenceId,
                    ReferenceName = x.ReferenceName,
                }))).ToList();
        }
        protected void OnReferenceChange()
        {
            if (SelectedReferenceIds == null || !SelectedReferenceIds.Any())
            {
                OnChange.InvokeAsync(null);
                return;
            }
            SelectedReferences = AvailableItemReferencesForSelection
                .Where(w => SelectedReferenceIds.Contains(w.ReferenceId))
                .OrderBy(o => o.Item.Line.LineName).ThenBy(o => o.Item.ItemName).ThenBy(o => o.ReferenceName)
                .ToList();
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
            if (SelectedLineIds == null || !SelectedLineIds.Any())
            {
                SelectedItemIds = new List<int>();
                return;
            }
            SelectedItemIds = (from a in SelectedItemIds
                               join b in AvailableItemReferencesForSelection on a equals b.ItemId
                               join c in SelectedLineIds on b.Item.LineId equals c
                               select a).ToList();

        }
        void CleanReferences()
        {
            References = new List<GroupReferenceData>();
            if (SelectedItemIds == null || !SelectedItemIds.Any())
            {
                SelectedReferenceIds = new List<int>();
                SelectedReferences = new List<ServiceModel.ItemReference>();
                return;
            }

            SelectedReferenceIds = (from a in SelectedReferenceIds
                                    join b in AvailableItemReferencesForSelection on a equals b.ReferenceId
                                    join c in SelectedItemIds on b.ItemId equals c 
                                    select a).ToList();

            SelectedReferences = (from a in SelectedReferences
                                  join b in SelectedReferenceIds on a.ReferenceId equals b
                                  select a).ToList();
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
    }
}