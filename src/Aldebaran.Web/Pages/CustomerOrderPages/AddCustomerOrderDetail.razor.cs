using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderDetail
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
        public ICollection<CustomerOrderDetail> customerOrderDetails { get; set; }

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected CustomerOrderDetail customerOrderDetail;
        protected InventoryQuantities QuantitiesPanel;

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (customerOrderDetails.Any(ad => ad.REFERENCE_ID.Equals(customerOrderDetail.REFERENCE_ID)))
                    throw new Exception("La Referencia seleccionada, ya existe dentro de esta reserva.");

                var reference = await AldebaranDbService.GetItemReferences(new Query { Filter = "i=> i.REFERENCE_ID==@0", FilterParameters = new object[] { customerOrderDetail.REFERENCE_ID }, Expand = "Item" });
                customerOrderDetail.ItemReference = reference.Single();
                DialogService.Close(customerOrderDetail);
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

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            customerOrderDetail.REFERENCE_ID = reference?.REFERENCE_ID ?? 0;

            await QuantitiesPanel.Refresh(reference);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerOrderDetail = new CustomerOrderDetail();

            await base.SetParametersAsync(parameters);
        }

    }
}