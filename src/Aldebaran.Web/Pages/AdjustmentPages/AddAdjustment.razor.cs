using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AddAdjustment
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
        public AldebaranDbService AldebaranDbService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            adjustment = new Models.AldebaranDb.Adjustment();

            adjustmentReasonsForADJUSTMENTREASONID = await AldebaranDbService.GetAdjustmentReasons();

            adjustmentTypesForADJUSTMENTTYPEID = await AldebaranDbService.GetAdjustmentTypes();

            aspnetusersForASPNETUSERID = await AldebaranDbService.GetAspNetUsers();
        }

        protected bool errorVisible;

        protected Models.AldebaranDb.Adjustment adjustment;

        protected IEnumerable<Models.AldebaranDb.AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;

        protected IEnumerable<Models.AldebaranDb.AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;

        protected IEnumerable<Models.AldebaranDb.Aspnetuser> aspnetusersForASPNETUSERID;

        protected bool isSubmitInProgress;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateAdjustment(adjustment);
                DialogService.Close(adjustment);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}