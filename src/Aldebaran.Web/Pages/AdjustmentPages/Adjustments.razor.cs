using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Aldebaran.Web.Pages.AdjustmentPages;

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

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Adjustment> adjustments;

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.Adjustment> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"i => i.ASPNETUSER_ID.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Aspnetuser" });
        }
        protected override async Task OnInitializedAsync()
        {
            adjustments = await AldebaranDbService.GetAdjustments(new Query { Filter = $@"i => i.ASPNETUSER_ID.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "AdjustmentReason,AdjustmentType,Aspnetuser" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddAdjustment>("Add Adjustment", null);
            await grid0.Reload();
        }

        protected async Task EditRow(Aldebaran.Web.Models.AldebaranDb.Adjustment args)
        {
            await DialogService.OpenAsync<EditAdjustment>("Edit Adjustment", new Dictionary<string, object> { {"ADJUSTMENT_ID", args.ADJUSTMENT_ID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Adjustment adjustment)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteAdjustment(adjustment.ADJUSTMENT_ID);

                    if (deleteResult != null)
                    {
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
                    Detail = $"Unable to delete Adjustment"
                });
            }
        }

        protected Aldebaran.Web.Models.AldebaranDb.Adjustment adjustment;
        protected async Task GetChildData(Aldebaran.Web.Models.AldebaranDb.Adjustment args)
        {
            adjustment = args;
            var AdjustmentDetailsResult = await AldebaranDbService.GetAdjustmentDetails(new Query { Filter = $@"i => i.ADJUSTMENT_ID == {args.ADJUSTMENT_ID}", Expand = "Adjustment,ItemReference,Warehouse" });
            if (AdjustmentDetailsResult != null)
            {
                args.AdjustmentDetails = AdjustmentDetailsResult.ToList();
            }
        }

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.AdjustmentDetail> AdjustmentDetailsDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task AdjustmentDetailsAddButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Adjustment data)
        {
            var dialogResult = await DialogService.OpenAsync<AddAdjustmentDetail>("Add AdjustmentDetails", new Dictionary<string, object> { {"ADJUSTMENT_ID" , data.ADJUSTMENT_ID} });
            await GetChildData(data);
            await AdjustmentDetailsDataGrid.Reload();
        }

        protected async Task AdjustmentDetailsRowSelect(Aldebaran.Web.Models.AldebaranDb.AdjustmentDetail args, Aldebaran.Web.Models.AldebaranDb.Adjustment data)
        {
            var dialogResult = await DialogService.OpenAsync<EditAdjustmentDetail>("Edit AdjustmentDetails", new Dictionary<string, object> { {"ADJUSTMENT_DETAIL_ID", args.ADJUSTMENT_DETAIL_ID} });
            await GetChildData(data);
            await AdjustmentDetailsDataGrid.Reload();
        }

        protected async Task AdjustmentDetailsDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.AdjustmentDetail adjustmentDetail)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteAdjustmentDetail(adjustmentDetail.ADJUSTMENT_DETAIL_ID);

                    await GetChildData(adjustment);

                    if (deleteResult != null)
                    {
                        await AdjustmentDetailsDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
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