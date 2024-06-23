using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.CustomerReservationPages
{
    public partial class CustomerReservationDetails
    {
        #region Injections

        [Inject]
        protected ILogger<CustomerReservationDetails> Logger { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ICustomerService CustomerService { get; set; }

        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        protected IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected ICustomerReservationService CustomerReservationService { get; set; }

        [Inject]
        protected ICustomerReservationDetailService CustomerReservationDetailService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }


        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public int CustomerReservationId { get; set; }

        #endregion

        #region Global Variables
        protected CustomerReservation customerReservation;
        protected DocumentType documentType;
        protected ICollection<CustomerReservationDetail> customerReservationDetails;
        protected ICollection<ReferencesWarehouse> referenceWarehouses;
        protected bool isLoadingInProgress;
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await Task.Yield();
                customerReservation = await CustomerReservationService.FindAsync(CustomerReservationId);
                customerReservationDetails = (await CustomerReservationDetailService.GetByCustomerReservationIdAsync(customerReservation.CustomerReservationId)).ToList();

                foreach (var item in customerReservationDetails)
                {
                    item.ItemReference.ReferencesWarehouses = (await ReferencesWarehouseService.GetByReferenceIdAsync(item.ReferenceId)).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CustomerReservationDetails.OnInitializedAsync()");
            }
            finally { isLoadingInProgress = false; }
        }
        #endregion

        #region Events

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);
        
        #endregion
    }
}
