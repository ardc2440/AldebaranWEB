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
    public partial class Items
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

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Item> items;

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.Item> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            items = await AldebaranDbService.GetItems(new Query { Filter = $@"i => i.INTERNAL_REFERENCE.Contains(@0) || i.ITEM_NAME.Contains(@0) || i.PROVIDER_REFERENCE.Contains(@0) || i.PROVIDER_ITEM_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "MeasureUnit,Currency,MeasureUnit1,Line" });
        }
        protected override async Task OnInitializedAsync()
        {
            items = await AldebaranDbService.GetItems(new Query { Filter = $@"i => i.INTERNAL_REFERENCE.Contains(@0) || i.ITEM_NAME.Contains(@0) || i.PROVIDER_REFERENCE.Contains(@0) || i.PROVIDER_ITEM_NAME.Contains(@0) || i.NOTES.Contains(@0)", FilterParameters = new object[] { search }, Expand = "MeasureUnit,Currency,MeasureUnit1,Line" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddItem>("Add Item", null);
            await grid0.Reload();
        }

        protected async Task EditRow(DataGridRowMouseEventArgs<Aldebaran.Web.Models.AldebaranDb.Item> args)
        {
            await DialogService.OpenAsync<EditItem>("Edit Item", new Dictionary<string, object> { {"ITEM_ID", args.Data.ITEM_ID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Item item)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItem(item.ITEM_ID);

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
                    Detail = $"Unable to delete Item"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await AldebaranDbService.ExportItemsToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "MeasureUnit,Currency,MeasureUnit1,Line",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Items");
            }

            if (args == null || args.Value == "xlsx")
            {
                await AldebaranDbService.ExportItemsToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "MeasureUnit,Currency,MeasureUnit1,Line",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "Items");
            }
        }

        protected Aldebaran.Web.Models.AldebaranDb.Item item;
        protected async Task GetChildData(Aldebaran.Web.Models.AldebaranDb.Item args)
        {
            item = args;
            var ItemReferencesResult = await AldebaranDbService.GetItemReferences(new Query { Filter = $@"i => i.ITEM_ID == {args.ITEM_ID}", Expand = "Item" });
            if (ItemReferencesResult != null)
            {
                args.ItemReferences = ItemReferencesResult.ToList();
            }
        }

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.ItemReference> ItemReferencesDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task ItemReferencesAddButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.Item data)
        {
            var dialogResult = await DialogService.OpenAsync<AddItemReference>("Add ItemReferences", new Dictionary<string, object> { {"ITEM_ID" , data.ITEM_ID} });
            await GetChildData(data);
            await ItemReferencesDataGrid.Reload();
        }

        protected async Task ItemReferencesRowSelect(DataGridRowMouseEventArgs<Aldebaran.Web.Models.AldebaranDb.ItemReference> args, Aldebaran.Web.Models.AldebaranDb.Item data)
        {
            var dialogResult = await DialogService.OpenAsync<EditItemReference>("Edit ItemReferences", new Dictionary<string, object> { {"REFERENCE_ID", args.Data.REFERENCE_ID} });
            await GetChildData(data);
            await ItemReferencesDataGrid.Reload();
        }

        protected async Task ItemReferencesDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.ItemReference itemReference)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemReference(itemReference.REFERENCE_ID);

                    await GetChildData(item);

                    if (deleteResult != null)
                    {
                        await ItemReferencesDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete ItemReference"
                });
            }
        }
    }
}