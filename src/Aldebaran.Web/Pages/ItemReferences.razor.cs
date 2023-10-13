using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Aldebaran.Web.Pages
{
    public partial class ItemReferences
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

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.ItemReference> itemReferences;

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.ItemReference> grid0;
        protected bool isEdit = true;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = $@"i => i.REFERENCE_CODE.Contains(@0) || i.INTERNAL_REFERENCE_CODE.Contains(@0) || i.REFERENCE_NAME.Contains(@0) || i.PROVIDER_REFERENCE_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Item" });
        }
        protected override async Task OnInitializedAsync()
        {
            itemReferences = await AldebaranDbService.GetItemReferences(new Query { Filter = $@"i => i.REFERENCE_CODE.Contains(@0) || i.INTERNAL_REFERENCE_CODE.Contains(@0) || i.REFERENCE_NAME.Contains(@0) || i.PROVIDER_REFERENCE_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Item" });

            itemsForITEMID = await AldebaranDbService.GetItems();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            itemReference = new Aldebaran.Web.Models.AldebaranDb.ItemReference();
        }

        protected async Task EditRow(Aldebaran.Web.Models.AldebaranDb.ItemReference args)
        {
            isEdit = true;
            itemReference = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.ItemReference itemReference)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemReference(itemReference.REFERENCE_ID);

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
                    Detail = $"Unable to delete ItemReference"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await AldebaranDbService.ExportItemReferencesToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Item",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ItemReferences");
            }

            if (args == null || args.Value == "xlsx")
            {
                await AldebaranDbService.ExportItemReferencesToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Item",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ItemReferences");
            }
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ItemReference itemReference;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Item> itemsForITEMID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await AldebaranDbService.UpdateItemReference(itemReference.REFERENCE_ID, itemReference) : await AldebaranDbService.CreateItemReference(itemReference);

            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {

        }
    }
}