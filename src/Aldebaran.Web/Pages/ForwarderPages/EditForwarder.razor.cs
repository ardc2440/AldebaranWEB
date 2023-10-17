using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class EditForwarder
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

        [Parameter]
        public int FORWARDER_ID { get; set; }

        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            forwarder = await AldebaranDbService.GetForwarderByForwarderId(FORWARDER_ID);
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.UpdateForwarder(FORWARDER_ID, forwarder);
                DialogService.Close(true);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }
        protected async Task LocalizationHandler(Aldebaran.Web.Models.AldebaranDb.City city)
        {
            forwarder.CITY_ID = city?.CITY_ID ?? 0;
        }
        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}