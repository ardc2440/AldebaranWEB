using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.PurchaseOrderApprovalRangePages
{
    public partial class PurchaseOrderApprovalRanges
    {
        #region Injections

        [Inject]
        protected ILogger<PurchaseOrderApprovalRanges> Logger { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected IMapper Mapper { get; set; }

        [Inject]
        protected IPurchaseOrderApprovalRangeService PurchaseOrderApprovalRangeService { get; set; }

        #endregion

        #region Variables

        protected IEnumerable<PurchaseOrderApprovalRange> PurchaseOrderApprovalRangesList;

        protected LocalizedDataGrid<PurchaseOrderApprovalRange> PurchaseOrderApprovalRangesGrid;

        protected bool isLoadingInProgress;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await GetPurchaseOrderApprovalRangesAsync();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Methods

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        private async Task GetPurchaseOrderApprovalRangesAsync(
            CancellationToken ct = default)
        {
            await Task.Yield();

            PurchaseOrderApprovalRangesList = await PurchaseOrderApprovalRangeService.GetAsync(ct);            
        }

        #endregion

        #region Events

        protected async Task AddPurchaseOrderApprovalRange(
            MouseEventArgs args)
        {
            var result =
                await DialogService.OpenAsync<AddPurchaseOrderApprovalRange>("Nueva Tolerancia");

            if (result == true)
                NotificationService.Notify(new NotificationMessage { Summary = "Tolerancia", Severity = NotificationSeverity.Success, Detail = "Tolerancia creada correctamente." });

            await GetPurchaseOrderApprovalRangesAsync();
            await PurchaseOrderApprovalRangesGrid.Reload();
        }

        protected async Task EditPurchaseOrderApprovalRange(PurchaseOrderApprovalRange purchaseOrderApprovalRange)
        {
            var result =
                await DialogService.OpenAsync<EditPurchaseOrderApprovalRange>("Actualizar Tolerancia", new Dictionary<string, object> { { "PURCHASE_ORDER_APPROVAL_RANGE_ID", purchaseOrderApprovalRange.PurchaseOrderApprovalRangeId } });

            if (result == true)
                NotificationService.Notify(new NotificationMessage { Summary = "Tolerancia", Severity = NotificationSeverity.Success, Detail = "Tolerancia actualizada correctamente." });

            await GetPurchaseOrderApprovalRangesAsync();
            await PurchaseOrderApprovalRangesGrid.Reload();
        }

        #endregion
    }
}