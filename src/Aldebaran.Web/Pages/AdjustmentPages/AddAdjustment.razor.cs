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

        #endregion

        #region Parameters

        [Parameter]
        public string pAdjustmentId { get; set; } = "NoParamInput";

        #endregion

        #region Properties
        protected DateTime Now { get; set; }
        protected IEnumerable<AdjustmentReason> AdjustmentReasonsForAdjustmentReasonId { get; set; }
        protected IEnumerable<AdjustmentType> AdjustmentTypesForAdjustmentTypeId { get; set; }
        protected ICollection<AdjustmentDetail> AdjustmentDetails { get; set; }

        #endregion

        #region Global Variables

        protected bool errorVisible;
        protected string errorMessage;
        protected Adjustment adjustment;
        protected LocalizedDataGrid<AdjustmentDetail> adjustmentDetailGrid;
        protected bool isSubmitInProgress;
        protected DocumentType documentType;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            AdjustmentReasonsForAdjustmentReasonId = await AdjustmentReasonService.GetAsync();

            AdjustmentTypesForAdjustmentTypeId = await AdjustmentTypeService.GetAsync();

            Now = DateTime.UtcNow.AddDays(-1);

            AdjustmentDetails = new List<AdjustmentDetail>();

            _ = int.TryParse(pAdjustmentId, out int adjustmentId);

            adjustment = new Adjustment() { AdjustmentReason = null, AdjustmentType = null, Employee = null, StatusDocumentType = null };

            documentType = await DocumentTypeService.FindByCodeAsync("A");

            adjustment.StatusDocumentType = await StatusDocumentTypeService.FindByDocumentAndOrderAsync(documentType.DocumentTypeId, 1);
            adjustment.StatusDocumentTypeId = adjustment.StatusDocumentType.StatusDocumentTypeId;

            adjustment.Employee = await EmployeeService.FindByLoginUserIdAsync(Security.User.Id);
            adjustment.EmployeeId = adjustment.Employee.EmployeeId;
        }
        #endregion

        #region Events
        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!AdjustmentDetails.Any())
                    throw new Exception("No há ingresado ninguna referencia");

                adjustment.AdjustmentDetails = AdjustmentDetails;
                await AdjustmentService.AddAsync(adjustment);
                await DialogService.Alert("Ajuste guardado satisfactoriamente", "Información");
                NavigationManager.NavigateTo("adjustments");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                errorVisible = true;
            }
            finally { isSubmitInProgress = false; }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            if (await DialogService.Confirm("Está seguro que desea cancelar la creación del ajuste?", "Confirmar") == true)
                NavigationManager.NavigateTo("adjustments");
        }

        protected async Task AddAdjustmentDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Agregar referencia", new Dictionary<string, object> { { "AdjustmentDetails", AdjustmentDetails } });

            if (result == null)
                return;

            var detail = (AdjustmentDetail)result;

            AdjustmentDetails.Add(detail);

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
            var result = await DialogService.OpenAsync<EditAdjustmentDetail>("Modificar referencia", new Dictionary<string, object> { { "AdjustmentDetail", args } });
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