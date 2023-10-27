using Aldebaran.Web.Models;
using Aldebaran.Web.Pages.ForwarderPages;
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

        protected async Task Search(ChangeEventArgs args)
        {

            search = $"{args.Value}";

            await grid0.GoToPage(0);

            adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"I => i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Employee" });

        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                isLoadingInProgress = true;

                await Task.Yield();

                adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"i => i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Employee" });
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

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Adjustment adjustment)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea eliminar este ajuste?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteAdjustment(adjustment.ADJUSTMENT_ID);

                    if (deleteResult != null)
                    {
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
            var AdjustmentDetailsResult = await AldebaranDbService.GetAdjustmentDetails(new Query { Filter = $@"i => i.ADJUSTMENT_ID == {args.ADJUSTMENT_ID}", Expand = "Adjustment,ItemReference,Warehouse" });
            if (AdjustmentDetailsResult != null)
            {
                args.AdjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.AdjustmentDetail> AdjustmentDetailsDataGrid;

        protected async Task AdjustmentDetailsAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Adjustment data)
        {
            dialogResult = null;

            var result = await DialogService.OpenAsync<AddAdjustmentDetail>("Add AdjustmentDetails", new Dictionary<string, object> { { "ADJUSTMENT_ID", data.ADJUSTMENT_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Referencia agregada correctamente al ajuste." };
            }

            await GetChildData(data);
            await AdjustmentDetailsDataGrid.Reload();
        }

        protected async Task AdjustmentDetailsRowSelect(Models.AldebaranDb.AdjustmentDetail args, Models.AldebaranDb.Adjustment data)
        {
            var dialogResult = await DialogService.OpenAsync<EditAdjustmentDetail>("Edit AdjustmentDetails", new Dictionary<string, object> { { "ADJUSTMENT_DETAIL_ID", args.ADJUSTMENT_DETAIL_ID } });
            await GetChildData(data);
            await AdjustmentDetailsDataGrid.Reload();
        }

        protected async Task EditChildRow(Models.AldebaranDb.AdjustmentDetail args, Models.AldebaranDb.Adjustment data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<EditForwarderAgent>("Actualizar referencia", new Dictionary<string, object> { { "ADJUSTMENT_DETAIL_ID", args.ADJUSTMENT_DETAIL_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Referencia actualizada correctamente." };
            }
            await GetChildData(data);
            await AdjustmentDetailsDataGrid.Reload();
        }

        protected async Task AdjustmentDetailsDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.AdjustmentDetail adjustmentDetail)
        {
            try
            {
                dialogResult = null;

                if (await DialogService.Confirm("Esta seguro que desea eliminar esta referencia del ajuste actual?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteAdjustmentDetail(adjustmentDetail.ADJUSTMENT_DETAIL_ID);

                    await GetChildData(adjustment);

                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Referencia eliminada del ajuste correctamente." };
                        await AdjustmentDetailsDataGrid.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete AdjustmentDetail"
                });
            }
        }
    }
}