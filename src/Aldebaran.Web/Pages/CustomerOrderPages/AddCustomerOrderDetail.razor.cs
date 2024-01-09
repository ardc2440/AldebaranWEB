using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerOrderPages
{
    public partial class AddCustomerOrderDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected CustomerOrderDetail customerOrderDetail;
        protected InventoryQuantities quantitiesPanel;
        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID { get; set; } = new List<ItemReference>();

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            itemReferencesForREFERENCEID = await ItemReferenceService.GetByStatusAsync(true);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerOrderDetail = new CustomerOrderDetail();

            await base.SetParametersAsync(parameters);
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (CustomerOrderDetails.Any(ad => ad.ReferenceId.Equals(customerOrderDetail.ReferenceId)))
                    throw new Exception("La Referencia seleccionada, ya existe dentro de esta reserva.");

                var reference = await ItemReferenceService.FindAsync(customerOrderDetail.ReferenceId);
                customerOrderDetail.ItemReference = reference;
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
            customerOrderDetail.ReferenceId = reference?.ReferenceId ?? 0;

            await quantitiesPanel.Refresh(reference);
        }
        #endregion
    }
}