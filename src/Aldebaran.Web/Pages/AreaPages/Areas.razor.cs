using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Aldebaran.Web.Models;

namespace Aldebaran.Web.Pages.AreaPages
{
    public partial class Areas
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

        protected IEnumerable<Models.AldebaranDb.Area> areas;

        protected RadzenDataGrid<Models.AldebaranDb.Area> grid0;

        protected string search = "";
        protected DialogResult dialogResult { get; set; }
        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            areas = await AldebaranDbService.GetAreas(new Query { Filter = $@"i => i.AREA_CODE.Contains(@0) || i.AREA_NAME.Contains(@0) || i.DESCRIPTION.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            areas = await AldebaranDbService.GetAreas(new Query { Filter = $@"i => i.AREA_CODE.Contains(@0) || i.AREA_NAME.Contains(@0) || i.DESCRIPTION.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.Area area)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteArea(area.AREA_ID);

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
                    Detail = $"Unable to delete Area"
                });
            }
        }

        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await AldebaranDbService.ExportAreasToCSV(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Areas");
            }

            if (args == null || args.Value == "xlsx")
            {
                await AldebaranDbService.ExportAreasToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter) ? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Areas");
            }
        }

        protected Models.AldebaranDb.Area area;
        protected async Task GetChildData(Models.AldebaranDb.Area args)
        {
            area = args;
            var ItemsAreasResult = await AldebaranDbService.GetItemsAreas(new Query { Filter = $@"i => i.AREA_ID == {args.AREA_ID}", Expand = "Area,Item.MeasureUnit,Item.Currency,Item.MeasureUnit1,Item.Line" });
            if (ItemsAreasResult != null)
            {
                args.ItemsAreas = ItemsAreasResult.ToList();
            }
        }

        protected RadzenDataGrid<Models.AldebaranDb.ItemsArea> ItemsAreasDataGrid;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task ItemsAreasAddButtonClick(MouseEventArgs args, Models.AldebaranDb.Area data)
        {
            dialogResult = null;
            var result = await DialogService.OpenAsync<AddItemsArea>("Agregar artículo", new Dictionary<string, object> { { "AREA_ID", data.AREA_ID } });
            if (result == true)
            {
                dialogResult = new DialogResult { Success = true, Message = "Artículo agregado correctamente al área." };
            }
            await GetChildData(data);
            await ItemsAreasDataGrid.Reload();
        }

        protected async Task ItemsAreasDeleteButtonClick(MouseEventArgs args, Models.AldebaranDb.ItemsArea itemsArea)
        {
            try
            {
                dialogResult = null;
                if (await DialogService.Confirm("Está seguro que desea eliminar esta artículo del área??") == true)
                {
                    var deleteResult = await AldebaranDbService.DeleteItemsArea(itemsArea.ITEM_ID, itemsArea.AREA_ID);
                    await GetChildData(area);
                    if (deleteResult != null)
                    {
                        dialogResult = new DialogResult { Success = true, Message = "Artículo eliminado del área correctamente." };
                        await ItemsAreasDataGrid.Reload();
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
    }
}