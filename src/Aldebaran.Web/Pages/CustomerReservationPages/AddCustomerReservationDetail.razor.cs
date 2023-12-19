using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using System.Linq.Dynamic.Core;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class AddCustomerReservationDetail
    {
        #region Injections
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected IItemReferenceService ItemReferenceService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public ICollection<CustomerReservationDetail> CustomerReservationDetails { get; set; }

        #endregion

        #region Global Variables
        protected bool errorVisible;
        protected string alertMessage;
        protected bool isSubmitInProgress;
        protected CustomerReservationDetail customerReservationDetail;
        protected InventoryQuantities QuantitiesPanel;
        protected IEnumerable<ItemReference> itemReferencesForREFERENCEID;
        #endregion

        #region Overrides
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerReservationDetail = new CustomerReservationDetail();
            itemReferencesForREFERENCEID = await ItemReferenceService.GetByStatusAsync(true);

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

                if (CustomerReservationDetails.Any(ad => ad.ReferenceId.Equals(customerReservationDetail.ReferenceId)))
                    throw new Exception("La Referencia seleccionada, ya existe dentro de esta reserva.");

                var reference = await ItemReferenceService.FindAsync(customerReservationDetail.ReferenceId);
                customerReservationDetail.ItemReference = reference;
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

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            customerReservationDetail.ReferenceId = reference?.ReferenceId ?? 0;
            await QuantitiesPanel.Refresh(reference);
        }
        #endregion
    }
}