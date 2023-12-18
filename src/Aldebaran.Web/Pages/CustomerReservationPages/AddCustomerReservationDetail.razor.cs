using Aldebaran.Web.Models.AldebaranDb;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using System.Linq.Dynamic.Core;
using ServiceModel = Aldebaran.Application.Services.Models;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservationDetail
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
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected CustomerReservationDetail customerReservationDetail;
        protected InventoryQuantities QuantitiesPanel;
        protected IEnumerable<ServiceModel.ItemReference> itemReferencesForREFERENCEID;

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (CustomerReservationDetails.Any(ad => ad.REFERENCE_ID.Equals(customerReservationDetail.REFERENCE_ID)))
                    throw new Exception("La Referencia seleccionada, ya existe dentro de esta reserva.");

                var reference = await AldebaranDbService.GetItemReferences(new Query { Filter = "i=> i.REFERENCE_ID==@0", FilterParameters = new object[] { customerReservationDetail.REFERENCE_ID }, Expand = "Item" });
                customerReservationDetail.ItemReference = reference.Single();
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
            //itemReferencesForREFERENCEID = await AldebaranDbService.GetItemReferences(new Query { Filter = "i => i.IS_ACTIVE && i.Item.IS_ACTIVE", Expand = "Item.Line" });

            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler(ServiceModel.ItemReference reference)
        {
            customerReservationDetail.REFERENCE_ID = reference?.ReferenceId ?? 0;
            //await QuantitiesPanel.Refresh(reference);
        }
    }
}