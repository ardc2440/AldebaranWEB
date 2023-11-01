using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class EditCustomerReservationDetail
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
        public CustomerReservationDetail customerReservationDetail { get; set; }

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
                DialogService.Close(customerReservationDetail);
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
            customerReservationDetail = new CustomerReservationDetail();

            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler()
        {
            await QuantitiesPanel.Refresh(customerReservationDetail.ItemReference);
        }
    }
}