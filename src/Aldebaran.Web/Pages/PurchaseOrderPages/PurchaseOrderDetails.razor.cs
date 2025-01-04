using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Shared;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderPages
{
    public partial class PurchaseOrderDetails
    {
        #region Injections
        [Inject]
        protected ILogger<PurchaseOrderDetails> Logger { get; set; }

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
        protected IPurchaseOrderService PurchaseOrderService { get; set; }

        [Inject]
        protected IPurchaseOrderDetailService PurchaseOrderDetailService { get; set; }

        [Inject]
        protected IReferencesWarehouseService ReferencesWarehouseService { get; set; }


        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public int PurchaseOrderId { get; set; }

        #endregion

        #region Global Variables
        protected PurchaseOrder PurchaseOrder;
        protected DocumentType documentType;
        protected ICollection<PurchaseOrderDetail> PurchaseOrderDetailList;
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
                PurchaseOrder = await PurchaseOrderService.FindAsync(PurchaseOrderId);
                PurchaseOrderDetailList = (await PurchaseOrderDetailService.GetByPurchaseOrderIdAsync(PurchaseOrder.PurchaseOrderId)).ToList();

                foreach (var item in PurchaseOrderDetailList)
                {
                    item.ItemReference.ReferencesWarehouses = (await ReferencesWarehouseService.GetByReferenceIdAsync(item.ReferenceId)).ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PurchaseOrderDetails.OnInitializedAsync()");
            }
            finally { isLoadingInProgress = false; }
        }
        #endregion

        #region Events

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"[{reference.Item.InternalReference}] {reference.Item.ItemName} - {reference.ReferenceName}";

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion
    }
}
