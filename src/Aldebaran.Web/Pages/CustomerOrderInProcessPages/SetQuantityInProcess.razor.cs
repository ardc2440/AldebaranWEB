using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderInProcessPages
{
    public partial class SetQuantityInProcess
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
        public DetailInProcess detailInProcess { get; set; }

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected InventoryQuantities QuantitiesPanel;

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (await DialogService.Confirm("Está seguro que desea enviar a proceso esta cantidad de la referencia?", "Confirmar") == true)
                    DialogService.Close(detailInProcess);
            }
            catch (Exception ex)
            {
                alertMessage = ex.Message;
                errorVisible = true;
            }
            finally
            {
                isSubmitInProgress = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler()
        {
            await QuantitiesPanel.Refresh(detailInProcess.REFERENCE_ID);
        }

    }
}