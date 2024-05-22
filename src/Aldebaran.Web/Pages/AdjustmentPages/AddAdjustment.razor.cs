using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Aldebaran.Web.Resources.LocalizedControls;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AddAdjustment
    {
        #region Injections

        [Inject]
        protected ILogger<AddAdjustment> Logger { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IAdjustmentReasonService AdjustmentReasonService { get; set; }
        [Inject]
        protected IAdjustmentTypeService AdjustmentTypeService { get; set; }
        [Inject]
        protected IDocumentTypeService DocumentTypeService { get; set; }
        [Inject]
        protected IStatusDocumentTypeService StatusDocumentTypeService { get; set; }
        [Inject]
        protected IEmployeeService EmployeeService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        protected IAdjustmentService AdjustmentService { get; set; }
        [Inject]
        protected TooltipService TooltipService { get; set; }

        #endregion

        #region Parameters

        #endregion

        #region Properties
        protected IEnumerable<AdjustmentReason> AdjustmentReasonsForAdjustmentReasonId { get; set; }
        protected IEnumerable<AdjustmentType> AdjustmentTypesForAdjustmentTypeId { get; set; }
        protected ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        #endregion

        #region Global Variables

        protected Adjustment adjustment;
        protected LocalizedDataGrid<AdjustmentDetail> adjustmentDetailGrid;
        protected DocumentType documentType;
        protected bool IsErrorVisible;
        private bool Submitted = false;
        protected bool IsSubmitInProgress;
        protected bool isLoadingInProgress;
        protected string Error;
        protected int lastReferenceId = 0;
        protected short lastWarehouseId = 0;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;
                AdjustmentReasonsForAdjustmentReasonId = await AdjustmentReasonService.GetAsync();
                AdjustmentTypesForAdjustmentTypeId = await AdjustmentTypeService.GetAsync();
                AdjustmentDetails = new List<AdjustmentDetail>();
                adjustment = new Adjustment() { AdjustmentReason = null, AdjustmentType = null, Employee = null, StatusDocumentType = null };
                documentType = await DocumentTypeService.FindByCodeAsync("A");
                adjustment.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
                adjustment.StatusDocumentTypeId = adjustment.StatusDocumentType.StatusDocumentTypeId;
                adjustment.Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
                adjustment.EmployeeId = adjustment.Employee.EmployeeId;
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }
        #endregion

        #region Events
        void ShowTooltip(ElementReference elementReference, string content, TooltipOptions options = null) => TooltipService.Open(elementReference, content, options);

        protected async Task<string> GetReferenceHint(ItemReference reference) => $"({reference.Item.Line.LineName}) {reference.Item.ItemName} - {reference.ReferenceName}";

        protected async Task FormSubmit()
        {
            try
            {
                Submitted = true;
                IsSubmitInProgress = true;
                if (!AdjustmentDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                adjustment.AdjustmentDetails = AdjustmentDetails;
                var result = await AdjustmentService.AddAsync(adjustment);
                NavigationManager.NavigateTo($"adjustments/{result.AdjustmentId}");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, nameof(FormSubmit));
                IsErrorVisible = true;
                Error = ex.Message;
            }
            finally { IsSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación del ajuste?", "Confirmar") == true)
                NavigationManager.NavigateTo("adjustments");
        }

        protected async Task AddAdjustmentDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Agregar referencia", new Dictionary<string, object> {
                    { "AdjustmentDetails", AdjustmentDetails },
                    { "LastReferenceId", lastReferenceId },
                    { "LastWarehouseId", lastWarehouseId} });

            if (result == null)
                return;

            var detail = (AdjustmentDetail)result;
            AdjustmentDetails.Add(detail);
            lastReferenceId = detail.ReferenceId;
            lastWarehouseId = detail.WarehouseId;
            await adjustmentDetailGrid.Reload();
        }

        protected async Task DeleteAdjustmentDetailButtonClick(MouseEventArgs args, AdjustmentDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                AdjustmentDetails.Remove(item);
                await adjustmentDetailGrid.Reload();
            }
        }

        protected async Task EditRow(AdjustmentDetail args)
        {
            var result = await DialogService.OpenAsync<EditAdjustmentDetail>("Actualizar referencia", new Dictionary<string, object> { { "AdjustmentDetail", args } });
            if (result == null)
                return;
            var detail = (AdjustmentDetail)result;

            AdjustmentDetails.Remove(args);
            AdjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }
        #endregion
    }
}