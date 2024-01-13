using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Models.ViewModels;
using Aldebaran.Web.Resources.LocalizedControls;
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
                await GetAdjustmentsAsync();
                await DialogResultResolver();
            }
            finally
            {
                isLoadingInProgress = false;
            }
        }

        #endregion

        #region Events
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

            if (Action == "edit")
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Summary = "Ajuste de inventario",
                    Severity = NotificationSeverity.Success,
                    Detail = $"El ajuste ha sido actualizada correctamente."
                });
                return;
            }

            NotificationService.Notify(new NotificationMessage
            {
                Summary = "Ajuste de inventario",
                Severity = NotificationSeverity.Success,
                Detail = $"El ajuste ha sido creada correctamente."
            });
        }

        async Task GetAdjustmentsAsync(string searchKey = null, CancellationToken ct = default)
        {
            await Task.Yield();
            adjustments = string.IsNullOrEmpty(searchKey) ? await AdjustmentService.GetAsync(ct) : await AdjustmentService.GetAsync(searchKey, ct);
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

        protected async Task EditAdjustment(Adjustment args)
        {
            NavigationManager.NavigateTo("edit-adjustment/" + args.AdjustmentId);
        }

        protected bool CanEditAdjustment(Adjustment args)
        {
            return Security.IsInRole("Admin", "Adjustment Editor") && args.StatusDocumentType.EditMode;
        }

        protected async Task CancelAdjustmentClick(MouseEventArgs args, Adjustment adjustment)
        {
            try
            {
                DialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea cancelar este ajuste?") == true)
                {
                    adjustment.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 2);
                    adjustment.StatusDocumentTypeId = adjustment.StatusDocumentType.StatusDocumentTypeId;
                    adjustment.AdjustmentDetails = (await AdjustmentDetailService.GetByAdjustmentIdAsync(adjustment.AdjustmentId)).ToList();
                    await AdjustmentService.CancelAsync(adjustment.AdjustmentId);
                    await GetAdjustmentsAsync();
                    NotificationService.Notify(new NotificationMessage
                    {
                        Summary = "Ajuste de inventario",
                        Severity = NotificationSeverity.Success,
                        Detail = $"Ajuste de inventario ha sido cancelado correctamente."
                    });
                    await AdjustmentsGrid.Reload();
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"No se ha podido eliminar el ajuste"
                });
            }
        }

        protected async Task GetChildData(Adjustment args)
        {
            adjustment = args;

            var AdjustmentDetailsResult = await AdjustmentDetailService.GetByAdjustmentIdAsync(args.AdjustmentId);

            if (AdjustmentDetailsResult != null)
            {
                adjustmentDetails = AdjustmentDetailsResult.ToList();

                args.AdjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        #endregion

    }
}