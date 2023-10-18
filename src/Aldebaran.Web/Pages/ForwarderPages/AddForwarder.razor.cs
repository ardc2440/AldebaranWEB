using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.ForwarderPages
{
    public partial class AddForwarder
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

        [Inject]
        protected SecurityService Security { get; set; }

        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.Forwarder forwarder;
        protected bool isSubmitInProgress;

        protected override async Task OnInitializedAsync()
        {
            forwarder = new Aldebaran.Web.Models.AldebaranDb.Forwarder();
        }
        
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                await AldebaranDbService.CreateForwarder(forwarder);
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