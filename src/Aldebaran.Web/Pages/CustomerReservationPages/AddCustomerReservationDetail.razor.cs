using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;

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
        public ICollection<CustomerReservationDetail> customerReservationDetails { get; set; }

        protected bool errorVisible;

        protected string alertMessage;

        protected bool isSubmitInProgress;

        protected IEnumerable<Models.AldebaranDb.Adjustment> customerReservationsForCUSTOMERRESERVATIONID;

        protected IEnumerable<Models.AldebaranDb.ItemReference> itemReferencesForREFERENCEID;

        protected CustomerReservationDetail customerReservationDetail;

        protected override async Task OnInitializedAsync()
        {
            customerReservationsForCUSTOMERRESERVATIONID = await AldebaranDbService.GetAdjustments();

            itemReferencesForREFERENCEID = await AldebaranDbService.GetItemReferences();
        }

        protected async Task FormSubmit()
        {
            try
            {
                errorVisible = false;
                isSubmitInProgress = true;

                if (customerReservationDetails.Any(ad => ad.REFERENCE_ID.Equals(customerReservationDetail.REFERENCE_ID)))
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

        bool hasCUSTOMER_RESERVATION_IDValue;

        [Parameter]
        public int CUSTOMER_RESERVATION_ID { get; set; }

        bool hasREFERENCE_IDValue;

        [Parameter]
        public int REFERENCE_ID { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            customerReservationDetail = new CustomerReservationDetail();

            hasCUSTOMER_RESERVATION_IDValue = parameters.TryGetValue<int>("CUSTOMER_RESERVATION_ID", out var hasCUSTOMER_RESERVATION_IDResult);

            if (hasCUSTOMER_RESERVATION_IDValue)
            {
                customerReservationDetail.CUSTOMER_RESERVATION_ID = hasCUSTOMER_RESERVATION_IDResult;
            }

            hasREFERENCE_IDValue = parameters.TryGetValue<int>("REFERENCE_ID", out var hasREFERENCE_IDResult);

            if (hasREFERENCE_IDValue)
            {
                customerReservationDetail.REFERENCE_ID = hasREFERENCE_IDResult;
            }

            await base.SetParametersAsync(parameters);
        }

        protected async Task ItemReferenceHandler(ItemReference reference)
        {
            customerReservationDetail.REFERENCE_ID = reference?.REFERENCE_ID ?? 0;
        }
    }
}