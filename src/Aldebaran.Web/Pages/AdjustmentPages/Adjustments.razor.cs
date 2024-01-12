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

        protected LocalizedDataGrid<Adjustment> grid0;

        protected DialogResult DialogResult { get; set; }

        protected string search = "";

        protected bool isLoadingInProgress;

        protected DocumentType documentType;

        protected Adjustment adjustment;

        protected LocalizedDataGrid<AdjustmentDetail> AdjustmentDetailsDataGrid;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                documentType = await DocumentTypeService.FindByCodeAsync("A");

                adjustments = await AdjustmentService.GetAsync(search);
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        #endregion

        #region Events

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            adjustments = await AdjustmentService.GetAsync(search);

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-adjustment");
        }

        protected async Task EditRow(Adjustment args)
        {
            NavigationManager.NavigateTo("edit-adjustment/" + args.AdjustmentId);
        }

        protected bool CanEdit(Adjustment args)
        {
            return Security.IsInRole("Admin", "Adjustment Editor") && args.StatusDocumentType.EditMode;
        }

        protected async Task GridCancelButtonClick(MouseEventArgs args, Adjustment adjustment)
        {
            try
            {
                DialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar este ajuste?") == true)
                {
                    adjustment.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 2);

                    adjustment.StatusDocumentTypeId = adjustment.StatusDocumentType.StatusDocumentTypeId;

                    adjustment.AdjustmentDetails = (await AdjustmentDetailService.GetByAdjustmentIdAsync(adjustment.AdjustmentId)).ToList();

                    await AdjustmentService.UpdateAsync(adjustment.AdjustmentId, adjustment);

                    await DialogService.Alert($"Ajuste cancelado correctamente", "Información");

                    await grid0.Reload();
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
                args.AdjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        #endregion

    }
}