using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class AddAdjustment
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

        protected DateTime Now { get; set; }

        protected bool errorVisible;

        protected string errorMessage;

        protected Models.AldebaranDb.Adjustment adjustment;

        protected IEnumerable<Models.AldebaranDb.AdjustmentReason> adjustmentReasonsForADJUSTMENTREASONID;

        protected IEnumerable<Models.AldebaranDb.AdjustmentType> adjustmentTypesForADJUSTMENTTYPEID;

        protected IEnumerable<Models.AldebaranDb.Employee> employeesForEMPLOYEEID;

        protected ICollection<AdjustmentDetail> adjustmentDetails;

        protected AdjustmentDetail adjustmentDetail;

        protected RadzenDataGrid<AdjustmentDetail> adjustmentDetailGrid;

        protected bool isSubmitInProgress;

        protected RadzenPanelMenu panelMenu;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            adjustment = new Models.AldebaranDb.Adjustment();

            adjustmentReasonsForADJUSTMENTREASONID = await AldebaranDbService.GetAdjustmentReasons();

            adjustmentTypesForADJUSTMENTTYPEID = await AldebaranDbService.GetAdjustmentTypes();

            Now = DateTime.UtcNow.AddDays(-1);

            adjustmentDetails = new List<AdjustmentDetail>();

            adjustment = new Adjustment();

            adjustment.EMPLOYEE_ID = 1;
        }

        protected async Task FormSubmit()
        {
            try
            {
                isSubmitInProgress = true;
                if (!adjustmentDetails.Any())
                    throw new Exception("No ha ingresado ninguna referencia");

                adjustment.AdjustmentDetails = adjustmentDetails;
                await AldebaranDbService.CreateAdjustment(adjustment);
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
            var result = await DialogService.OpenAsync<EditAdjustmentDetail>("Actualizar referencia", new Dictionary<string, object> { { "adjustmentDetail", args } });
            if (result == null)
                return;
            var detail = (AdjustmentDetail)result;

            adjustmentDetails.Remove(args);
            adjustmentDetails.Add(detail);

            await adjustmentDetailGrid.Reload();
        }
    }
}