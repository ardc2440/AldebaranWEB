using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
using Aldebaran.Web.Shared;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class Adjustments
    {
        #region Injections

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }
        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }
        [Inject]
        protected IAdjustmentService AdjustmentService { get; set; }
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
        [Inject]
        protected IAdjustmentDetailService AdjustmentDetailService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }

        #endregion

        #region Global Variables

        protected IEnumerable<Adjustment> adjustments;
        protected IEnumerable<AdjustmentDetail> adjustmentDetails;

        protected LocalizedDataGrid<Adjustment> AdjustmentsGrid;
        protected DialogResult DialogResult { get; set; }
        protected string search = "";
        protected bool isLoadingInProgress;
        protected DocumentType documentType;
        protected Adjustment adjustment;

        protected int skip = 0;
        protected int top = 0;
        protected int count = 0;

        #endregion

        #region Parameters

        [Parameter]
        public string ADJUSTMENT_ID { get; set; } = null;
        [Parameter]
        public string Action { get; set; } = null;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                await Task.Yield();
                documentType = await DocumentTypeService.FindByCodeAsync("A");
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events

        protected async Task LoadData(LoadDataArgs args)
        {
            skip = args.Skip.Value;
            top = args.Top.Value;
            await GetAdjustmentsAsync();
        }

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        async Task DialogResultResolver(CancellationToken ct = default)
        {
            if (ADJUSTMENT_ID == null)
                return;

            var valid = int.TryParse(ADJUSTMENT_ID, out var adjustmentId);
            if (!valid)
                return;

            var adjustment = await AdjustmentService.FindAsync(adjustmentId, ct);
            if (adjustment == null)
                return;
            
            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Ajuste de inventario",
                Severity = NotificationSeverity.Success,
                Detail = $"El ajuste ha sido creado correctamente con el consecutivo {adjustmentId}."
            });
        }

        async Task GetAdjustmentsAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            (adjustments, count) = string.IsNullOrEmpty(searchKey) ? await AdjustmentService.GetAsync(skip, top, ct) : await AdjustmentService.GetAsync(skip, top, searchKey, ct);
        }

        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";
            await AdjustmentsGrid.GoToPage(0);
            await GetAdjustmentsAsync(search);
        }

        protected async Task AddAdjustmentClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-adjustment");
        }

        protected async Task GetChildData(Adjustment args)
        {
            adjustment = args;

            var adjustmentDetailsResult = await AdjustmentDetailService.GetByAdjustmentIdAsync(args.AdjustmentId);

            if (adjustmentDetailsResult != null)
            {
                adjustmentDetails = adjustmentDetailsResult.ToList();

                args.AdjustmentDetails = adjustmentDetailsResult.ToList();
            }
        }

        private async Task ShowImageDialog(string articleName) => DialogService.Open<ImageDialog>("", new Dictionary<string, object>
            {
                { "ArticleName", articleName }
            });
        #endregion

    }
}