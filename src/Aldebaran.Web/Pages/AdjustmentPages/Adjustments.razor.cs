using Aldebaran.Web.Models;
using Aldebaran.Web.Models.AldebaranDb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages.AdjustmentPages
{
    public partial class Adjustments
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

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.AldebaranDb.Adjustment> adjustments;

        protected RadzenDataGrid<Models.AldebaranDb.Adjustment> grid0;

        protected DialogResult dialogResult { get; set; }

        protected string search = "";

        protected bool isLoadingInProgress;

        protected DocumentType documentType;

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"I => i.AdjustmentReason.ADJUSTMENT_REASON_NAME.Contains(@0) || i.AdjustmentType.ADJUSTMENT_TYPE_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Employee" });

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                documentType = await AldebaranDbService.GetDocumentTypeByCode("A");

                adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"i => i.AdjustmentReason.ADJUSTMENT_REASON_NAME.Contains(@0) || i.AdjustmentType.ADJUSTMENT_TYPE_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Employee,StatusDocumentType.DocumentType" });
            }
            finally
            {
                isLoadingInProgress = false;
            }

        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            NavigationManager.NavigateTo("add-adjustment");
        }

        protected async Task EditRow(Models.AldebaranDb.Adjustment args)
        {
            NavigationManager.NavigateTo("edit-adjustment/" + args.ADJUSTMENT_ID);
        }

        protected bool CanEdit(Adjustment args)
        {
            return Security.IsInRole("Admin", "Adjustment Editor") && args.StatusDocumentType.EDIT_MODE;
        }

        protected async Task GridCancelButtonClick(MouseEventArgs args, Models.AldebaranDb.Adjustment adjustment)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea cancelar este ajuste?") == true)
                {
                    var statusCanceled = await AldebaranDbService.GetStatusDocumentTypeByDocumentAndOrder(documentType, 2);
                    var deleteResult = await AldebaranDbService.CancelAdjustment(adjustment, statusCanceled.STATUS_DOCUMENT_TYPE_ID);

                    if (deleteResult != null)
                    {
                        adjustment.StatusDocumentType = statusCanceled;
                        adjustment.STATUS_DOCUMENT_TYPE_ID = adjustment.StatusDocumentType.STATUS_DOCUMENT_TYPE_ID;

                        dialogResult = new DialogResult { Success = true, Message = "Ajuste eliminado correctamente." };
                        await grid0.Reload();
                    }
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

        protected Models.AldebaranDb.Adjustment adjustment;

        protected async Task GetChildData(Models.AldebaranDb.Adjustment args)
        {
            adjustment = args;
            var AdjustmentDetailsResult = await AldebaranDbService.GetAdjustmentDetails(new Query { Filter = $@"i => i.ADJUSTMENT_ID == {args.ADJUSTMENT_ID}", Expand = "Adjustment,ItemReference,Warehouse, ItemReference.Item" });
            if (AdjustmentDetailsResult != null)
            {
                args.AdjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.AdjustmentDetail> AdjustmentDetailsDataGrid;

    }
}