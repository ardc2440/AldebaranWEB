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
    public partial class ItemsAreas
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

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.ItemsArea> itemsAreas;

        protected RadzenDataGrid<Aldebaran.Web.Models.AldebaranDb.ItemsArea> grid0;
        protected bool isEdit = true;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            itemsAreas = await AldebaranDbService.GetItemsAreas(new Query { Expand = "Item" });
        }
        protected override async Task OnInitializedAsync()
        {
            itemsAreas = await AldebaranDbService.GetItemsAreas(new Query { Expand = "Item" });

            itemsForITEMID = await AldebaranDbService.GetItems();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEdit = false;
            itemsArea = new Aldebaran.Web.Models.AldebaranDb.ItemsArea();
        }

        protected async Task EditRow(Aldebaran.Web.Models.AldebaranDb.ItemsArea args)
        {
            isEdit = true;
            itemsArea = args;
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Aldebaran.Web.Models.AldebaranDb.ItemsArea itemsArea)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemsArea(itemsArea.ITEM_ID, itemsArea.AREA_ID);

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
                    Detail = $"Unable to delete ItemsArea"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await AldebaranDbService.ExportItemsAreasToCSV(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Item",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ItemsAreas");
            }

            if (args == null || args.Value == "xlsx")
            {
                await AldebaranDbService.ExportItemsAreasToExcel(new Query
{
    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
    OrderBy = $"{grid0.Query.OrderBy}",
    Expand = "Item",
    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
}, "ItemsAreas");
            }
        }
        protected bool errorVisible;
        protected Aldebaran.Web.Models.AldebaranDb.ItemsArea itemsArea;

        protected IEnumerable<Aldebaran.Web.Models.AldebaranDb.Item> itemsForITEMID;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task FormSubmit()
        {
            try
            {
                var result = isEdit ? await AldebaranDbService.UpdateItemsArea(itemsArea.ITEM_ID, itemsArea.AREA_ID, itemsArea) : await AldebaranDbService.CreateItemsArea(itemsArea);

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