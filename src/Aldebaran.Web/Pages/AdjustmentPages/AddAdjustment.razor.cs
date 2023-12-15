using Aldebaran.Application.Services;
using Aldebaran.Application.Services.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

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
        public IAdjustmentReasonService AdjustmentReasonService { get; set; }

        [Inject]
        public IAdjustmentTypeService AdjustmentTypeService { get; set; }

        [Inject]
        public IDocumentTypeService DocumentTypeService { get; set; }

        [Inject]
        public IStatusDocumentTypeService StatusDocumentTypeService { get; set; }

        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }

        [Inject]
        public IAdjustmentService AdjustmentService { get; set; }

        #endregion

        #region Parameters

        [Parameter]
        public string pAdjustmentId { get; set; } = "NoParamInput";

        #endregion

        #region Global Variables

        protected DateTime Now { get; set; }

        protected bool errorVisible;

        protected string errorMessage;

        protected Adjustment adjustment;

        protected IEnumerable<AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;

        protected IEnumerable<AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;

        protected IEnumerable<Employee> employeesForEMPLOYEEID;

        protected ICollection<AdjustmentDetail> adjustmentDetails;

        protected RadzenDataGrid<AdjustmentDetail> adjustmentDetailGrid;

        protected bool isSubmitInProgress;

        protected DocumentType documentType;

        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            adjustmentReasonsForADJUSTMENTREASONID = await AdjustmentReasonService.GetAsync();

            adjustmentTypesForADJUSTMENTTYPEID = await AdjustmentTypeService.GetAsync();

            Now = DateTime.UtcNow.AddDays(-1);

            adjustmentDetails = new List<AdjustmentDetail>();

            var adjustmentId = 0;

            int.TryParse(pAdjustmentId, out adjustmentId);

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
                if (!adjustmentDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                adjustment.AdjustmentDetails = adjustmentDetails;
                await AdjustmentService.AddAsync(adjustment);
                await DialogService.Alert("Ajuste Guardado Satisfactoriamente", "Información");
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
            if (await DialogService.Confirm("Está seguro que cancelar la creacion del Ajuste??", "Confirmar") == true)
                NavigationManager.NavigateTo("adjustments");
        }

        protected async Task AddAdjustmentDetailButtonClick(MouseEventArgs args)
        {
            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Nueva referencia", new Dictionary<string, object> { { "adjustmentDetails", adjustmentDetails } });

            if (result == null)
                return;

            var detail = (AdjustmentDetail)result;

            adjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }

        protected async Task DeleteAdjustmentDetailButtonClick(MouseEventArgs args, AdjustmentDetail item)
        {
            if (await DialogService.Confirm("Está seguro que desea eliminar esta referencia?", "Confirmar") == true)
            {
                adjustmentDetails.Remove(item);

                await adjustmentDetailGrid.Reload();
            }
        }

        protected async Task EditRow(AdjustmentDetail args)
        {
            var result = await DialogService.OpenAsync<EditAdjustmentDetail>("Actualizar referencia", new Dictionary<string, object> { { "pAdjustmentDetail", args } });
            if (result == null)
                return;
            var detail = (AdjustmentDetail)result;

            adjustmentDetails.Remove(args);
            adjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }
        #endregion
    }
}