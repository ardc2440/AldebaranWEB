﻿using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.ViewModel;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.ReportPages.Freezone_vs_Available.Components
{
    public partial class FreezoneVsAvailableReportFilter
    {
        #region Injections
        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public FreezoneVsAvailableFilter Filter { get; set; } = new();
        #endregion

        #region Variables
        protected bool IsErrorVisible;
        protected bool IsSubmitInProgress;
        protected MultiReferencePicker referencePicker;
        protected List<ItemReference> SelectedReferences = new List<ItemReference>();
        protected List<ItemReference> AvailableItemReferencesForSelection = new();
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Filter ??= new FreezoneVsAvailableFilter();
            var references = (await ItemReferenceService.GetReportsReferencesAsync(isReferenceActive: true, isItemActive: true, isExternalInventory: true)).ToList();
            AvailableItemReferencesForSelection = references;
            referencePicker.SetAvailableItemReferencesForSelection(AvailableItemReferencesForSelection);
        }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (Filter?.ItemReferences?.Any() == true)
            {
                referencePicker.SetSelectedItemReferences(Filter.ItemReferences.Select(s => s.ReferenceId).ToList());
            }
            StateHasChanged();
        }
        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                IsSubmitInProgress = true;
                Filter.ItemReferences = SelectedReferences;
                DialogService.Close(Filter);
            }
            catch (Exception ex)
            {
                IsErrorVisible = true;
            }
            finally
            {
                IsSubmitInProgress = false;
            }
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
        protected async Task ItemReferenceHandler(List<ItemReference> references)
        {
            SelectedReferences = references ?? new List<ItemReference>();
        }
        #endregion
    }
}
